using System;

namespace ApiCoreEcommerce.Entities
{
    public interface ITimeStampedModel
    {
        long Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}