using System;
using BoilerController.Api.Contracts;

namespace BoilerController.Api.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Returns true if object is null
        /// </summary>
        /// <param name="entity">IEntity object</param>
        /// <returns>True if null; false otherwise</returns>
        public static bool IsObjectNull(this IEntity entity)
        {
            return entity == null;
        }

        /// <summary>
        /// Returns true if object is uninitialized
        /// </summary>
        /// <param name="entity">IEntity object</param>
        /// <returns>true if uninitialized; false otherwise</returns>
        public static bool IsEmptyObject(this IEntity entity)
        {
            return entity.Id == Guid.Empty;
        }
    }
}
