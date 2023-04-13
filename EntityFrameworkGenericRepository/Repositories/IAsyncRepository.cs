using EntityFrameworkGenericRepository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// Async repository interface that contains basic async methods that should be available to use in all implementations.<br/>
/// Basic implementations can be found in <see cref="BaseAsyncRepository{TEntity,TId,TContext}"/>
///</summary>
/// <remarks>
/// <para>
/// See <see cref="IRepository{TEntity,TId}"/> for a sync repository with the same methods.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">Entity saved in the repository.</typeparam>
/// <typeparam name="TId">Id entity type for the repository. Can be a simple <see cref="int"/>, or a complex class.</typeparam>
/// <seealso cref="BaseAsyncRepository{TContext, Tid, TEntity}"/>
/// <seealso cref="IRepository{TEntity,TId}"/>
/// <seealso cref="IAsyncPagedRepository{TEntity,TId,TFilter}"/>
public interface IAsyncRepository<TEntity, in TId> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    /// <summary>
    /// Returns the only entity with the given id or default value if no such entity exists.<br/>
    /// Throws an exception if more than one entity has the same id.<br/>
    /// Related entities are foreign keys that need to be included with <see cref="EntityFrameworkQueryableExtensions.Include{TEntity, TProperty}"/>
    /// The method should be implemented with <see cref="EntityFrameworkQueryableExtensions.FirstOrDefaultAsync{TSource}(System.Linq.IQueryable{TSource},System.Threading.CancellationToken)"/><br/>
    /// </summary>
    /// <param name="id">The id of the entity</param>
    /// <param name="includeRelatedEntities"> include related entities if true, else false</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="T:System.InvalidOperationException">More than one entity has id of <paramref name="id"/></exception>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous FindById operation. The task results contains the only entity with the given id, or default(TEntity) if no such entity is found.</returns>
    Task<TEntity?> FindByIdAsync(TId id, bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all the entities of <typeparamref name="TEntity"/>.
    /// Related entities are foreign keys that need to be included with<see cref = "EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/>
    /// The method should be implemented with <see cref="Enumerable.ToList{TSource}"/>
    /// </summary>
    /// <param name="includeRelatedEntities">Include related entities if true, else false.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous FindAll operation. The task results contains a list of all entities of <typeparamref name="TEntity"/></returns>
    Task<ICollection<TEntity>> FindAllAsync(bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities with an id contained in <paramref name="ids"/>.<br/>
    /// Related entities are foreign keys that need to be included with <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/>
    /// This method should be implemented with <see cref="Enumerable.ToList{TSource}"/>
    /// </summary>
    /// <param name="ids">The id of the entity</param>
    /// <param name="includeRelatedEntities">Include related entities if true, else false.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous FindAllById operation. The task results contains a list of all entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/> </returns>
    Task<ICollection<TEntity>> FindAllByIdAsync(IEnumerable<TId> ids, bool includeRelatedEntities = INCLUDE, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all entities of <typeparamref name="TEntity"/>
    /// </summary>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous CountAll operation. The task results contains the amount of entities of <typeparamref name="TEntity"/></returns>
    Task<long> CountAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts all entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/>
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to count by.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous CountAllById operation. The task results contains the amount of entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/> </returns>
    Task<long> CountAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if entity exists by id.
    /// </summary>
    /// <param name="id">a <typeparam name="TId"/> to test if exists in the table.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous ExistsById operation. The task results contains true if the entity exists, else false.</returns>
    Task<bool> ExistsByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if there is an entity for each id in ids.
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to test if exists in the table.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous ExistsAllById operation. The task results contains true if all the ids have an entity, else false.</returns>
    Task<bool> ExistsAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if at least one of the ids has an entity.
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to test if exists in the table.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous ExistsAnyById operation. The task results contains true if at least one of the ids has an entity, else false.</returns>
    Task<bool> ExistsAnyByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves and commits an entity to the DB.
    /// Use <see cref="UpdateAsync"/> to update an existing entity. This method will throw an exception when saving an entity with an existing id.
    /// </summary>
    /// <param name="entity">The entity to be saved.</param>
    /// <exception cref="Exceptions.EntityWithSameIdExistsException">If the entity already exists.</exception>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous Save operation. The task results contains the saved entity.</returns>
    /// <seealso cref="UpdateAsync"/>
    /// <seealso cref="SaveAllAsync"/>
    Task<TEntity?> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves and commits multiple entities to the DB.<br/>
    /// Use <see cref="UpdateAllAsync"/> to update existing entities. This method will throw an exception when saving an entity with an existing id.
    /// </summary>
    /// <param name="entities">The entities to be saved.</param>
    /// <exception cref="Exceptions.EntityWithSameIdExistsException">If one or more entities already exist.</exception>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous SaveAll operation. The task results contains the saved entities.</returns>
    /// <seealso cref="SaveAsync"/>
    /// <seealso cref="UpdateAllAsync"/>
    Task<ICollection<TEntity>> SaveAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates and commits an entity to the DB.
    /// Use <see cref="SaveAsync"/> to save a new entity. This method will throw an exception when trying to update a non existing entity.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <exception cref="Exceptions.EntityIdDoesNotExistsException">if the entity is missing.</exception>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous Update operation. The task results contains the updated entity.</returns>
    /// <seealso cref="SaveAsync"/>
    /// <seealso cref="UpdateAllAsync"/>
    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates and commits multiple entities to the DB.<br/>
    /// Use <see cref="SaveAllAsync"/> to save new entities. This method will throw an exception when trying to update a non existing entity.
    /// </summary>
    /// <param name="entities">The entities to be updated.</param>
    /// <exception cref="Exceptions.EntityIdDoesNotExistsException">if the entity is missing.</exception>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous UpdateAll operation. The task results contains the updated entities.</returns>>
    /// <seealso cref="UpdateAsync"/>
    /// <seealso cref="SaveAllAsync"/>
    Task<ICollection<TEntity>> UpdateAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple entities.
    /// </summary>
    /// <param name="entities">The entities to be deleted.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAllAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by id.
    /// </summary>
    /// <param name="id">The id of the entity to be deleted.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple entities by id.
    /// </summary>
    /// <param name="ids">The ids of the entities to be deleted.</param>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAllByIdAsync(IEnumerable<TId> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all the entities.
    /// </summary>
    /// <param name="cancellationToken">a <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">if the <see cref="cancellationToken"/> is canceled.</exception>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}