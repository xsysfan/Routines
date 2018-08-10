﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DashboardCode.Routines.Json
{
    public class SerializerOptions
    {
        public SerializerOptions(
             Delegate serializer,
             Func<StringBuilder, bool> nullSerializer,
             bool handleNullProperty,
             InternalNodeOptions internalNodeOptions)
        {
            Serializer = serializer;
            NullSerializer = nullSerializer;
            HandleNullProperty = handleNullProperty;
            InternalNodeOptions = internalNodeOptions;
        }
        public readonly Delegate Serializer;
        public readonly Func<StringBuilder, bool> NullSerializer;
        public readonly bool HandleNullProperty;
        public readonly InternalNodeOptions InternalNodeOptions;
    }

    public class InternalNodeOptions
    {
        public InternalNodeOptions(
            bool handleEmptyObjectLiteral,
            bool handleEmptyArrayLiteral,
            Func<StringBuilder, bool> nullSerializer,
            bool handleNullProperty,
            Func<StringBuilder, bool> nullArraySerializer,
            bool handleNullArrayProperty,
            string serializationName
            )
        {
            HandleEmptyObjectLiteral = handleEmptyObjectLiteral;
            HandleEmptyArrayLiteral = handleEmptyArrayLiteral;
            NullSerializer = nullSerializer;
            HandleNullProperty = handleNullProperty;
            NullArraySerializer = nullArraySerializer;
            HandleNullArrayProperty = handleNullArrayProperty;
            SerializationName = serializationName;
        }

        public readonly bool HandleEmptyObjectLiteral = true;
        public readonly bool HandleEmptyArrayLiteral  = true;

        public readonly Func<StringBuilder, bool> NullSerializer; 
        public readonly bool HandleNullProperty = true;
        public readonly Func<StringBuilder, bool> NullArraySerializer;
        public readonly bool HandleNullArrayProperty = true;
        public readonly string SerializationName = null;
    }

    internal class SerializersPair
    {
        public ConstantExpression SerializerExpression { get; set; }
        public Func<StringBuilder, bool> NullSerializer { get; set; }
        public bool HandleNullProperty { get; set; }

        public SerializersPair(ConstantExpression serializerExpression, Func<StringBuilder, bool> nullSerializer, bool handleNullProperty)
        {
            this.SerializerExpression = serializerExpression;
            this.NullSerializer = nullSerializer;
            this.HandleNullProperty = handleNullProperty;
        }
    }

    public static class JsonChainTools
    {
        internal static bool IsNullable(Type type)
        {
            var @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
            return @value;
        }

        //internal static bool IsNullable(Type type, bool handleNullProperty)
        //{
        //    var @value = false;
        //    if (handleNullProperty)
        //    {
        //        @value = !type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(type) != null;
        //    }
        //    return @value;
        //}

        internal static SerializersPair CreateSerializsPair(
            ChainNode node, 
            //Type parentType, 
            Func<ChainNode, SerializerOptions> getLeafSerializerSet, 
            Func<ChainNode, InternalNodeOptions> getInternalSerializerSet,
            Func<LambdaExpression,Delegate> compile)
        {
            if (node.Children.Count == 0) // leaf node
            {
                var serializerOptions = getLeafSerializerSet(node);
                var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;
                var formatterDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), serializationType, typeof(bool));
                var serializerMethodInfo = serializerOptions.Serializer;
                var genericResolvedDelegate = serializerMethodInfo; // serializerMethodInfo.CreateDelegate(formatterDelegateType, null);
                var serializerExpression = Expression.Constant(genericResolvedDelegate, genericResolvedDelegate.GetType());
                return new SerializersPair(serializerExpression, serializerOptions.NullSerializer, serializerOptions.HandleNullProperty);
            }
            else // internal node
            {
                var internalSerializerSet = getInternalSerializerSet(node);
                var properies = new List<Expression>();
                foreach (var c in node.Children)
                {
                    var n = c.Value;
                    ConfigureSerializeProperty(n, node.Type, properies, getLeafSerializerSet, getInternalSerializerSet, compile);
                }
                var objectFormatterLambda = CreateSerializeObjectLambda(node.Type, internalSerializerSet.HandleEmptyObjectLiteral, properies.ToArray());
                var @delegate = compile(objectFormatterLambda);
                
                var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());
                return new SerializersPair(delegateConstant, internalSerializerSet.NullSerializer, internalSerializerSet.HandleNullProperty);
            }
        }

        public static void ConfigureSerializeProperty(ChainMemberNode node, Type parentType, List<Expression> propertyExpressions
            , Func<ChainNode, SerializerOptions> getSerialiazerOptions
            , Func<ChainNode, InternalNodeOptions> getInternalNodeOptions,
            Func<LambdaExpression, Delegate> compile)
        {

            bool? isNullableValueType = IsNullableStruct(node.Type);
            //var internalNodeOptions = getInternalNodeOptions(node,true);

            var getterLambdaExpression = node.Expression;
            var getterDelegate = compile(getterLambdaExpression);
            var getterConstantExpression = Expression.Constant(getterDelegate, getterDelegate.GetType());
            var serializationType = Nullable.GetUnderlyingType(node.Type) ?? node.Type;

            Type propertyType;
            //SerializersPair expressions;
            //bool isEnumerable = node is ChainEnumerablePropertyNode;

            ConstantExpression formatterExpression = null;
            ConstantExpression nullFormatterExpression = null;

            var internalNodeOptions = getInternalNodeOptions(node);

            if (node.IsEnumerable)
            {
                propertyType = typeof(IEnumerable<>).MakeGenericType(node.Type);//  ((ChainEnumerablePropertyNode)node).EnumerableType;
                // check that property should be serailizable: SerializeRefProperty
                

                var itemSerializers = CreateSerializsPair(node, /*parentType,*/ getSerialiazerOptions, getInternalNodeOptions, compile);

                //var itemSerializerExpression = itemSerializers.Item1;
                //var itemNullSerializerExpression = itemSerializers.Item2;
                var sbParameterExpression = Expression.Parameter(typeof(StringBuilder), "sb");
                var tParameterExpression = Expression.Parameter(propertyType, "t");

                ConstantExpression nullItemSerializerExpression = null;
                if (IsNullable(node.Type))
                {
                    if (itemSerializers.NullSerializer == null)
                        throw new NotSupportedException($"Null serializer is not setuped for leaf node '{node.FindLinkedRootXPath()}' ");
                    nullItemSerializerExpression = CreateSerializeNullConstant(itemSerializers.NullSerializer);
                }

                MethodCallExpression methodCallExpression = CreateSerializeArrayMethodCall(
                    serializationType,
                    isNullableValueType,
                    itemSerializers.SerializerExpression,
                    nullItemSerializerExpression, //itemSerializers.NullSerializer,
                    sbParameterExpression,
                    tParameterExpression,
                    internalNodeOptions.HandleEmptyArrayLiteral);

                var serializeArrayLambda = Expression.Lambda(methodCallExpression, new[] { sbParameterExpression, tParameterExpression });
                var serializeArrayDelegate = compile(serializeArrayLambda);

                formatterExpression = Expression.Constant(serializeArrayDelegate, serializeArrayDelegate.GetType());
                if (internalNodeOptions.HandleNullArrayProperty)
                    nullFormatterExpression = CreateSerializeNullConstant(internalNodeOptions.NullArraySerializer);
            }
            else
            {
                propertyType = serializationType;
                var serializersPair = CreateSerializsPair(node,/* parentType,*/ getSerialiazerOptions, getInternalNodeOptions, compile);
                formatterExpression = serializersPair.SerializerExpression;

                if (serializersPair.HandleNullProperty && IsNullable(node.Type))
                {
                    if (serializersPair.NullSerializer == null)
                        throw new NotSupportedException($"Null serializer is not setuped for internal node '{node.FindLinkedRootXPath()}' ");
                    nullFormatterExpression = CreateSerializeNullConstant(serializersPair.NullSerializer);
                }
                //nullFormatterExpression = expressions.NullSerializer;
            }
            bool isLeaf = node.Children.Count ==  0;
            MethodInfo propertySerializerMethodInfo = (node.IsEnumerable) ?
                GetEnumerablePropertySerializerMethodInfo(nullFormatterExpression != null) :
                GetPropertySerializerMethodInfo(isLeaf, isNullableValueType, nullFormatterExpression != null);

            var propertySerializationName = internalNodeOptions?.SerializationName ?? node.MemberName;

            var serializePropertyExpression = CreateSerializePropertyLambda(
                         parentType,
                         propertyType,

                         propertySerializationName,

                         getterConstantExpression,
                         propertySerializerMethodInfo,
                         formatterExpression,
                         nullFormatterExpression);

            var @delegate = compile(serializePropertyExpression);
            var delegateConstant = Expression.Constant(@delegate, @delegate.GetType());

            propertyExpressions.Add(delegateConstant);
        }

        #region Expressions 
        internal static ConstantExpression CreateSerializeNullConstant(Func<StringBuilder, bool> nullSerializer)
        {
            var constantExpression = Expression.Constant(nullSerializer, typeof(Func<StringBuilder, bool>));
            return constantExpression;
        }

        private static LambdaExpression CreateSerializeObjectLambda(Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            var sbExpression = Expression.Parameter(typeof(StringBuilder), "sb");
            var tExpression = Expression.Parameter(objectType, "t");
            var serializeObjectMethodCallExpression =  CreateSerializeMethodCallExpression( sbExpression,  tExpression,  objectType,  handleEmptyPropertyList, serializeProperties);
            var objectFormatterLambda = Expression.Lambda(serializeObjectMethodCallExpression, new[] { sbExpression, tExpression });
            return objectFormatterLambda;
        }

        internal static MethodCallExpression CreateSerializeMethodCallExpression(ParameterExpression sbExpression, ParameterExpression tExpression, Type objectType, bool handleEmptyPropertyList, Expression[] serializeProperties)
        {
            MethodInfo serializeObjectGenericMethodInfo;
            if (handleEmptyPropertyList)
                serializeObjectGenericMethodInfo = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeAssociativeArrayHandleEmpty));
            else
                serializeObjectGenericMethodInfo = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeAssociativeArray));

            var serializeObjectResolvedMethodInfo = serializeObjectGenericMethodInfo.MakeGenericMethod(objectType);
            var serializePropertyFuncDelegateType = typeof(Func<,,>).MakeGenericType(typeof(StringBuilder), objectType, typeof(bool));

            var serializeObjectMethodCallExpression = Expression.Call(
                serializeObjectResolvedMethodInfo,
                new Expression[] { sbExpression, tExpression, Expression.NewArrayInit(serializePropertyFuncDelegateType, serializeProperties) }
            );
            return serializeObjectMethodCallExpression;
        }

        internal static bool? IsNullableStruct(Type type)
        {
            var @value = default(bool?);
            var nullableGenericType = Nullable.GetUnderlyingType(type);
            if (nullableGenericType != null)
                @value = true;
            else if (type.GetTypeInfo().IsValueType)
                @value = false;
            return @value;
        }

        internal static MethodCallExpression CreateSerializeArrayMethodCall(
            Type serializationType,
            bool? isNullableStruct,//SerializationPipeline propertyPipeline,
            ConstantExpression serializeExpression,
            ConstantExpression serializeNullExpression,
            ParameterExpression sbExpression,
            ParameterExpression tExpression,
            bool handleEmptyList
            )
        {
            bool itemNullSerializerRequired=true;
            MethodInfo serializePropertyMethod;
            // REM: my way to avoid (un)boxing ; other could be using something like this __refvalue(__makeref(v), int); 
            if (isNullableStruct == null)
            {
                if (handleEmptyList)
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefArrayHandleEmpty));
                else
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefArray));
            }
            else
            {
                if (isNullableStruct.Value)
                {
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValueArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValueArray));

                }
                else
                {
                    itemNullSerializerRequired = false;
                    if (handleEmptyList)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeValueArrayHandleEmpty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeValueArray));
                }
            }

            var serializePropertyGenericMethodInfo = serializePropertyMethod.MakeGenericMethod(serializationType);
            MethodCallExpression methodCallExpression;
            if (itemNullSerializerRequired)
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] { sbExpression, tExpression, serializeExpression, serializeNullExpression });
            else
                methodCallExpression = Expression.Call(serializePropertyGenericMethodInfo, new Expression[] { sbExpression, tExpression, serializeExpression });
            return methodCallExpression;
        }

        private static MethodInfo GetEnumerablePropertySerializerMethodInfo(bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (!handleNullProperty)
                serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefProperty));
            else
                serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull));
            return serializePropertyMethod;
        }

        private static MethodInfo GetPropertySerializerMethodInfo(bool isLeaf, bool? isNullableValueType, bool handleNullProperty)
        {
            MethodInfo serializePropertyMethod;
            if (isNullableValueType == null)
            {
                if (!handleNullProperty)
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefProperty));
                else
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeRefPropertyHandleNull));
            }
            else
            {
                if (isNullableValueType.Value && isLeaf)
                {
                    if (!handleNullProperty)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValueProperty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValuePropertyHandleNull));
                }
                else if (isNullableValueType.Value && !isLeaf)
                {
                    if (!handleNullProperty)
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValueNavProperty));
                    else
                        serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeNValueNavPropertyHandleNull));
                }
                else
                {
                    serializePropertyMethod = typeof(JsonComplexStringBuilderExtensions).GetTypeInfo().GetDeclaredMethod(nameof(JsonComplexStringBuilderExtensions.SerializeValueProperty));
                }
            }
            return serializePropertyMethod;
        }

        private static LambdaExpression CreateSerializePropertyLambda(
            Type entityType,
            Type propertyCanonicType,
            string serializationName,
            Expression getterExpression,
            MethodInfo serializePropertyMethodInfo,
            ConstantExpression serializeLeafExpression,
            ConstantExpression serializeNullExpression)
        {
            var sb = Expression.Parameter(typeof(StringBuilder), "sb");
            var t  = Expression.Parameter(entityType, "t");

            var serializePropertyGeneric  = serializePropertyMethodInfo.MakeGenericMethod(entityType, propertyCanonicType);
            var serializationNameConstant = Expression.Constant(serializationName, typeof(string));

            MethodCallExpression methodCallExpression;
            if (serializeNullExpression == null)
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeLeafExpression }
                );
            else
                methodCallExpression = Expression.Call(serializePropertyGeneric,
                    new Expression[] { sb, t, serializationNameConstant, getterExpression, serializeLeafExpression, serializeNullExpression }
                );
            var serializePropertyLambda = Expression.Lambda(methodCallExpression, new[] { sb, t });
            return serializePropertyLambda;
        }
        #endregion

        public static MethodInfo GetMethodInfoExpr<T>(Expression<Func<StringBuilder, T, bool>> expression, Func<LambdaExpression, Delegate> compile)
        {
            MethodInfo methodInfo = default(MethodInfo);
            if (expression.Body is MethodCallExpression callExpression)
            {
                var p0 = expression.Parameters[0];
                var p1 = expression.Parameters[1];
                var a0 = callExpression.Arguments[0];
                var a1 = callExpression.Arguments[1];
                if (p0 == a0 && p1 == a1)
                    methodInfo = callExpression.Method;
            }
            if (methodInfo == null)
            {
                var @delegate = compile(expression);
                methodInfo = @delegate.GetMethodInfo();
            }
            return methodInfo;
        }
    }
}
