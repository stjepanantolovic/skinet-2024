using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Specifications
{
    public class BaseSpecification<T> (Expression<Func<T, bool>>? criteria) : ISpecification<T>
    {
        protected BaseSpecification():this(null){}
        public Expression<Func<T, bool>>? Criteria => criteria;
    }
}