﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DashboardCode.Routines
{
    public interface IChainVisitor<TRootEntity>
    {
        void ParseRoot<TEntity>(Expression<Func<TRootEntity, TEntity>> getterExpression, string memberName);
        void ParseRootEnumerable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> getterExpression, string memberName);
        void Parse<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity>> getterExpression, bool changeCurrenNode, string memberName);
        void ParseEnumerable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> getterExpression, bool changeCurrenNode, string memberName);

        // TODO: Add navigation chain support to Nullable
        //void ParseRootNullable<TEntity>(Expression<Func<TRootEntity, TEntity?>> getterExpression) where TEntity : struct;
        //void ParseRootEnumerableNullable<TEntity>(Expression<Func<TRootEntity, IEnumerable<TEntity>>> getterExpression);
        //void ParseNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, TEntity?>> getterExpression) where TEntity : struct;
        //void ParseEnumerableNullable<TThenEntity, TEntity>(Expression<Func<TThenEntity, IEnumerable<TEntity>>> getterExpression);
    }
}