﻿namespace BeyondNet.Ddd.Es.Domain.Entities
{
    public interface IAggregateRootRepository
    {
       Task Add(SampleAggregateRoot entity);
    }
}
