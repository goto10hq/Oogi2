﻿using Microsoft.Azure.Cosmos;
using Oogi2.Entities;

namespace Oogi2.Queries
{
    public interface IQuery<T> where T : BaseEntity
    {
        QueryDefinition ToQueryDefinition(T item);
        QueryDefinition ToGetFirstOrDefault(T item);        
    }

    public interface IQuery 
    {
        QueryDefinition ToQueryDefinition();     
    }
}
