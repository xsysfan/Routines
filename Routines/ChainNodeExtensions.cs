﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DashboardCode.Routines
{
    public static class LeafRuleManager
    {
        public static readonly Func<ChainNode, IEnumerable<MemberInfo>> Default = ComposeLeafRule();
        public static readonly Func<ChainNode, IEnumerable<MemberInfo>> DefaultEfCore = ComposeLeafRule(isIncludeSpecialReadonly:true);

        public static Func<ChainNode, IEnumerable<MemberInfo>> ComposeLeafRule<T>(Include<T> include)
        {
            var root = IncludeExtensions.CreateChainNode(include);
            return ComposeLeafRule(root);
        }

        public static Func<ChainNode, IEnumerable<MemberInfo>> ComposeLeafRule(ChainNode root)
        {
            return (node) =>
            {
                var filteredProperties = new List<MemberInfo>();
                var path = ChainNodeTree.FindLinkedRootKeyPath(node);
                var n = default(ChainNode);
                if (path.Length == 0)
                {
                    n = root;
                }
                else
                {
                    if (ChainNodeTree.FindNode(root, path, out ChainMemberNode c))
                        n = c;
                }
                if (n != default(ChainNode))
                {
                    var type = n.Type;
                    foreach (var child in n.Children)
                    {
                        var propertyName = child.Key;
                        var propertyInfo = type.GetProperty(propertyName);
                        filteredProperties.Add(propertyInfo);
                    }
                }
                return filteredProperties;
            };
        }

        public static Func<ChainNode, IEnumerable<MemberInfo>> ComposeLeafRule(IReadOnlyCollection<Type> types=null, bool isIncludeSpecialReadonly=false)
        {
            return (node) =>
            {
                var type = node.Type;
                Func<Type, bool> isIncludable;
                if (types == null)
                    isIncludable = TypeExtensions.IsSystemType;
                else
                    isIncludable = (t) => types.Contains(t);
                var filteredMembers = new List<MemberInfo>();
                var typeInfo = type.GetTypeInfo();
                var properties = typeInfo.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                bool includeSpecialReadonly = false;
                if (isIncludeSpecialReadonly)
                {
                    includeSpecialReadonly = TypeExtensions.IsAnonymousType(type) || TypeExtensions.IsTuple(type);
                }
                foreach (var propertyInfo in properties)
                    if (propertyInfo.CanRead
                        && (includeSpecialReadonly || (!includeSpecialReadonly && propertyInfo.CanWrite))
                        && propertyInfo.GetIndexParameters().Length == 0)
                    {
                        var propertyType = propertyInfo.PropertyType;
                        if (isIncludable(propertyType))
                            filteredMembers.Add(propertyInfo);
                    }
                if (TypeExtensions.IsValueTuple(type))
                {
                    var fields = typeInfo.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var fieldInfo in fields)
                    {
                        var fieldType = fieldInfo.FieldType;
                        if (TypeExtensions.IsSystemType(fieldType))
                            filteredMembers.Add(fieldType);
                    }
                }
                return filteredMembers;
            };
        }
    }

    public static class ChainNodeExtensions
    {
        public static ChainMemberNode CloneChainMemberNode(this ChainMemberNode node, ChainNode parent)
        {
            var child = new ChainMemberNode(node.Type, node.Expression,/* node.MemberInfo,*/ node.MemberName, /*node.ChainMemberNodeExpressionType,*/ node.IsEnumerable, parent);
            parent.Children.Add(node.MemberName, child);
            return child;
        }

        public static bool HasLeafs(this ChainNode node)
        {
            bool @value = false;
            foreach (var n in node.Children.Values)
            {
                if (n.Children.Count()==0)
                {
                    @value = true;
                    break;
                }
            }
            return @value;
        }

        public static ChainMemberNode AddChild(this ChainNode node, string memberName)
        {
            var propertyInfo = node.Type.GetProperty(memberName);
            if (propertyInfo!=null)
                return AddChild(node, propertyInfo);
            else
            {
                var fieldInfo = node.Type.GetField(memberName);
                return AddChild(node, fieldInfo);
            }
        }

        public static ChainMemberNode AddChild(this ChainNode node, PropertyInfo propertyInfo)
        {
            var expression = node.Type.CreatePropertyLambda(propertyInfo);
            var child = new ChainMemberNode(propertyInfo.PropertyType, expression, propertyInfo.Name, isEnumerable: false, node);
            node.Children.Add(propertyInfo.Name, child);
            return child;
        }

        public static ChainMemberNode AddChild(this ChainNode node, FieldInfo fieldInfo)
        {
            var expression = node.Type.CreateFieldLambda(fieldInfo);
            var child = new ChainMemberNode(fieldInfo.FieldType, expression, fieldInfo.Name, isEnumerable:false, node);
            node.Children.Add(fieldInfo.Name, child);
            return child;
        }

        public static void AppendLeafs(this ChainNode node, Func<ChainNode, IEnumerable<MemberInfo>> leafRule =null)
        {
            if (leafRule == null)
                leafRule = LeafRuleManager.Default;
            foreach (var n in node.Children.Values)
                AppendLeafs(n, leafRule);
            var hasLeafs = node.HasLeafs();
            if (!hasLeafs)
            {
                var members = leafRule(node);
                foreach (var memberInfo in members)
                    node.AddChild(memberInfo.Name);
            }
        }

        public static Include<T> ComposeInclude<T>(this ChainNode root)
        {
            var parents = new ChainMemberNode[0];
            var entityType = root.Type;
            Type rootChainType = typeof(Chain<>).MakeGenericType(entityType);
            ParameterExpression tParameterExpression = Expression.Parameter(rootChainType, "t");
            int number = AddLevelRecursive(root, parents, 0, tParameterExpression, out Expression outExpression);
            Include<T> @destination = null;
            if (outExpression != null)
            {
                var lambdaExpression = Expression.Lambda<Include<T>>(outExpression, new[] { tParameterExpression });
                @destination = lambdaExpression.Compile();
            }
            return @destination;
        }

        private static int AddLevelRecursive(ChainNode root, ChainMemberNode[] parents, int number, Expression inExpression, out Expression outExpression)
        {
            var @value = number;
            var node = parents.Length == 0 ? root : parents[parents.Length - 1];
            outExpression = null;
            foreach (var childPair in node.Children)
            {
                var memberNode = childPair.Value;

                var modifiedParents = new ChainMemberNode[parents.Length + 1];
                parents.CopyTo(modifiedParents, 0);
                modifiedParents[parents.Length] = memberNode;

                if (memberNode.Children.Count != 0)
                {
                    @value = AddLevelRecursive(root, modifiedParents, @value, inExpression, out outExpression);
                    inExpression = outExpression;
                }
                else
                {
                    @value = AddMember(root, modifiedParents, @value, inExpression, out outExpression);
                    inExpression = outExpression;
                }
            }
            return @value;
        }

        private static int AddMember(ChainNode root, ChainMemberNode[] parents, int number, Expression inExpression, out Expression outExpression)
        {
            bool isRoot = true;
            outExpression = null;
            if (parents.Length == 0)
                throw new Exception("Impossible situation");
            var head = root;
            foreach (var p in parents)
            {
                MethodInfo includeMethodInfo = null;
                var isEnumerable = p.IsEnumerable;
                if (isRoot)
                {
                    if (isEnumerable)
                    {
                        Type rootChainType = typeof(Chain<>).MakeGenericType(root.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.IncludeAll));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    else
                    {
                        Type rootChainType = typeof(Chain<>).MakeGenericType(root.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(Chain<object>.Include));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    isRoot = false;
                }
                else
                {
                    if (isEnumerable)
                    {
                        Type rootChainType = typeof(ThenChain<,>).MakeGenericType(root.Type, head.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(ThenChain<object, object>.ThenIncludeAll));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                    else
                    {
                        Type rootChainType = typeof(ThenChain<,>).MakeGenericType(root.Type, head.Type);
                        MethodInfo includeGenericMethodInfo = rootChainType.GetTypeInfo().GetDeclaredMethod(nameof(ThenChain<object, object>.ThenInclude));
                        includeMethodInfo = includeGenericMethodInfo.MakeGenericMethod(p.Type);
                    }
                }
                var lambda = p.Expression;
                var name = p.MemberName;
                Expression pExp = Expression.Constant(name, typeof(string));
                var methodCallExpression = Expression.Call(inExpression, includeMethodInfo, new[] { lambda, pExp });
                inExpression = methodCallExpression;

                isRoot = false;
                head = p;
            }
            outExpression = inExpression;
            return number + 1;
        }

        public static IReadOnlyCollection<Type> ListLeafTypes(this ChainNode node)
        {
            var nodes = node.Children.Values;
            var types = new List<Type>();
            ListLeafTypesRecursive(nodes, types);
            return types;
        }

        public static IReadOnlyCollection<string> ListXPaths(this ChainNode node)
        {
            return ChainNodeTree.ListXPaths(node);
        }

        private static void ListLeafTypesRecursive(this IEnumerable<ChainMemberNode> nodes, List<Type> types)
        {
            foreach (var node in nodes)
            {
                var type = MemberExpressionExtensions.GetMemberType(((MemberExpression)node.Expression.Body));
                if (!types.Any(t => t.AssemblyQualifiedName == type.AssemblyQualifiedName))
                    types.Add(type);
                ListLeafTypesRecursive(node.Children.Values, types);
            }
        }

        private static object CloneItem(object sourceItem, ChainNode parentNode, IEnumerable<ChainMemberNode> nodes, Func<ChainNode, IEnumerable<MemberInfo>> leafRule)
        {
            if (sourceItem == null)
            {
                return null;
            }
            else
            {
                var type = sourceItem.GetType();
                var typeInfo = type.GetTypeInfo();
                if (sourceItem is string sourceString)
                {
                    return new String(sourceString.ToCharArray()); // String.Copy((string)sourceItem) absent in standard
                }
                else if (typeInfo.IsValueType)
                {
                    if (nodes.Count() > 0)
                        throw new InvalidOperationException($"It is impossible to clone value type's '{type.FullName}' instance specifing includes '{string.Join(", ", nodes.Select(e => e.MemberName))}' . Value type's instance can be cloned only entirely!");
                    return sourceItem;
                }
                else if (typeInfo.IsClass)
                {
                    var constructor = typeInfo.DeclaredConstructors.First(e => e.GetParameters().Count() == 0);
                    var destinationItem = constructor.Invoke(null);
                    CopyNodes(sourceItem, destinationItem, parentNode, nodes, leafRule);
                    return destinationItem;
                }
                else
                {
                    throw new InvalidOperationException($"Clonning of type '{type.FullName}' is not supported");
                }
            }
        }

        internal static void CopyNodes(
            object source,
            object destination,
            ChainNode parentNode,
            IEnumerable<ChainMemberNode> nodes,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule)
        {
            if (source is Array sourceArray)
            {
                CopyArray(sourceArray, (Array)destination, sourceItem => CloneItem(sourceItem, parentNode, nodes, leafRule));
            }
            else if (source is IEnumerable enumerable && destination is IList destinationList)
            {
                CopyList(enumerable, destinationList, sourceItem => CloneItem(sourceItem, parentNode, nodes, leafRule));
            }
            else if (source is IEnumerable sourceEnumerable && source.GetType().GetTypeInfo().ImplementedInterfaces.Any(t =>
                    t.GetTypeInfo().IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                CopySet(sourceEnumerable, destination, (sourceItem) => CloneItem(sourceItem, parentNode, nodes, leafRule));
            }
            else
            {

                if (leafRule != null)
                    CopyLeafs(source, destination, parentNode, leafRule);
                foreach (var node in nodes)
                {
                    var value = ((MemberExpression)node.Expression.Body).CopyMemberValue(source, destination);
                    var s = value.Item1;
                    var d = value.Item2;
                    if (s != null)
                        CopyNodes(s, d, node, node.Children.Values, leafRule); // recursion
                }
            }
        }

        public static void CopyLeafs(
            object source,
            object destination,
            ChainNode node,
            Func<ChainNode, IEnumerable<MemberInfo>> leafRule
            )
        {
            var members = leafRule(node);
            foreach (var memberInfo in members)
            {
                if (memberInfo is PropertyInfo propertyInfo) {
                    var value = propertyInfo.GetValue(source, null);
                    propertyInfo.SetValue(destination, value);
                }
                else if (memberInfo is FieldInfo fieldInfo)
                {
                    var value = fieldInfo.GetValue(source);
                    fieldInfo.SetValue(destination, value);
                }
                else
                {
                    throw new NotImplementedException("Unknown memberInfo type: " + memberInfo.GetType());
                }
            }
        }

        private static void CopyList(
            IEnumerable sourceEnumerable,
            IList destinationList,
            Func<object, object> copyItem)
        {
            destinationList.Clear();
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                destinationList.Add(destinationItem);
            }
        }

        private static void CopyArray(
            Array sourceArray,
            Array destinationArray,
            Func<object, object> copyItem)
        {
            if (destinationArray.Length != sourceArray.Length)
                throw new InvalidOperationException($"Destination array type of '{destinationArray.GetType().FullName}' is not null and its length differs from source array's length");
            for (int i = 0; i < sourceArray.Length; i++)
            {
                var sourceItem = sourceArray.GetValue(i);
                var destinationItem = copyItem(sourceItem);
                destinationArray.SetValue(destinationItem, i);
            }
        }

        private static void CopySet(
            IEnumerable sourceEnumerable,
            object set,
            Func<object, object> copyItem)
        {

            var getTypeInfo = set.GetType().GetTypeInfo();
            var clear = getTypeInfo.GetDeclaredMethod("Clear");
            var add = getTypeInfo.GetDeclaredMethod("Add");
            clear.Invoke(set, null);
            foreach (var sourceItem in sourceEnumerable)
            {
                var destinationItem = copyItem(sourceItem);
                add.Invoke(set, new[] { destinationItem });
            }
        }

        private static bool EqualsItem(object entity1Item, object entity2Item, IEnumerable<ChainMemberNode> nodes)
        {
            if (entity1Item == null && entity2Item == null)
                return true;
            else if ((entity1Item != null && entity2Item == null) || (entity1Item == null && entity2Item != null))
                return false;
            if (nodes.Count() > 0)
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else if (entity1Item is IEnumerable && !(entity1Item is string))
                return EqualsNodes(entity1Item, entity2Item, nodes);
            else
                return entity1Item.Equals(entity2Item);
        }

        internal static bool EqualsNodes(
            object entity1,
            object entity2,
            IEnumerable<ChainMemberNode> nodes)
        {
            bool @value = true;

            if (entity1 is Array entityArray1 && entity2 is Array entityArray2)
            {
                @value = EqualsArray(entityArray1, entityArray2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IList entityList1 && entity2 is IList entityList2 )
            {
                @value = EqualsList(entityList1, entityList2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else if (entity1 is IEnumerable && entity1.GetType().GetTypeInfo().ImplementedInterfaces.Any(t =>
                    t.GetTypeInfo().IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                @value = EqualsSet(entity1, entity2, (e1, e2) => EqualsItem(e1, e2, nodes));
            }
            else
            {
                foreach (var node in nodes)
                {
                    var values = ((MemberExpression)node.Expression.Body).GetMemberValues(entity1, entity2);
                    @value = EqualsItem(values.Item1, values.Item2, node.Children.Values);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }

        #region Collection's itearations
        private static bool EqualsList(
            IList entity1List,
            IList entity2List,
            Func<object, object, bool> equals)
        {
            var @value = true;
            if (entity1List.Count != entity2List.Count)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2List.Count; i++)
                {
                    var entity2Item = entity2List[i];
                    var entity1Item = entity1List[i];
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }

        private static bool EqualsSet(
            object set1,
            object set2,
            Func<object, object, bool> equals)
        {
            var setType = set1.GetType();
            var setTypeInfo = setType.GetTypeInfo();
            var countProperty = setTypeInfo.GetDeclaredProperty("Count");
            var count1 = (int)countProperty.GetValue(set1, null);
            var count2 = (int)countProperty.GetValue(set2, null);

            var genType = setType.GenericTypeArguments.First();
            var array1 = Array.CreateInstance(genType, count1);
            var array2 = Array.CreateInstance(genType, count2);
            var methods = setTypeInfo.GetDeclaredMethods("CopyTo");
            var сopyTo = methods
                .First(e => e.GetParameters().Count() == 1);
            сopyTo.Invoke(set1, new[] { array1 });
            сopyTo.Invoke(set2, new[] { array2 });
            var @value = EqualsArray(array1, array2, equals);
            return @value;
        }

        private static bool EqualsArray(
            Array entity1Array,
            Array entity2Array,
            Func<object, object, bool> equals
            )
        {
            var @value = true;
            if (entity1Array.Length != entity2Array.Length)
            {
                @value = false;
            }
            else
            {
                for (int i = 0; i < entity2Array.Length; i++)
                {
                    var entity2Item = entity2Array.GetValue(i);
                    var entity1Item = entity1Array.GetValue(i);
                    @value = equals(entity1Item, entity2Item);
                    if (@value == false)
                        break;
                }
            }
            return @value;
        }
        #endregion

        public static string FindLinkedRootXPath(this ChainNode node)
        {
            var @value = default(string);
            if (node is ChainMemberNode chainPropertyNode)
                @value = ChainNodeTree.FindLinkedRootXPath(chainPropertyNode);
            else
                @value = "/";
            return @value;
        }

        public static string[] FindLinkedRootKeyPath(this ChainMemberNode node)
        {
            return ChainNodeTree.FindLinkedRootKeyPath(node);
        }
    }
}