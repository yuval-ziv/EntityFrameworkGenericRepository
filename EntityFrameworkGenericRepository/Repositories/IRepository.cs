using EntityFrameworkGenericRepository.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkGenericRepository.Repositories;

/// <summary>
/// Repository interface that contains basic methods that should be available to use in all implementations.<br/>
/// Basic implementations can be found in <see cref="BaseRepository{TEntity,TId,TContext}"/>
///</summary>
/// <remarks>
/// <para>
/// See <see cref="IAsyncRepository{TEntity,TId}"/> for an async repository with the same methods.
/// </para>
/// </remarks>
/// <typeparam name="TEntity">Entity saved in the repository.</typeparam>
/// <typeparam name="TId">Id entity type for the repository. Can be a simple <see cref="int"/>, or a complex class</typeparam>
/// <seealso cref="BaseRepository{TEntity,TId,TContext}"/>
/// <seealso cref="IAsyncRepository{TEntity,TId}"/>
/// <seealso cref="IPagedRepository{TEntity,TId,TFilter}" />
public interface IRepository<TEntity, in TId> where TEntity : BaseEntity<TId> where TId : IEquatable<TId>
{
    private const bool INCLUDE = true;

    /// <summary>
    /// Returns the only entity with the given id or default value if no such entity exists.<br/>
    /// Throws an exception if more than one entity has the same id.<br/>
    /// Related entities are foreign keys that need to be included with <see cref="EntityFrameworkQueryableExtensions.Include{TEntity, TProperty}"/>
    /// The method should be implemented with <see cref="Queryable.SingleOrDefault{TSource}(System.Linq.IQueryable{TSource})"/><br/>
    /// </summary>
    /// <param name="id">The id of the entity</param>
    /// <param name="includeRelatedEntities"> include related entities if true, else false</param>
    /// <exception cref="T:System.InvalidOperationException">More than one entity has id of <paramref name="id"/></exception>
    /// <returns>The only entity with the given id, or default(TEntity) if no such entity is found.</returns>
    TEntity? FindById(TId id, bool includeRelatedEntities = INCLUDE);

    /// <summary>
    /// Returns all the entities of <typeparamref name="TEntity"/>.
    /// Related entities are foreign keys that need to be included with<see cref = "EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/>
    /// The method should be implemented with <see cref="Enumerable.ToList{TSource}"/>
    /// </summary>
    /// <param name="includeRelatedEntities">Include related entities if true, else false.</param>
    /// <returns>A list of all entities of <typeparamref name="TEntity"/></returns>
    ICollection<TEntity> FindAll(bool includeRelatedEntities = INCLUDE);

    /// <summary>
    /// Returns all entities with an id contained in <paramref name="ids"/>.<br/>
    /// Related entities are foreign keys that need to be included with <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/>
    /// This method should be implemented with <see cref="Enumerable.ToList{TSource}"/>
    /// </summary>
    /// <param name="ids">The id of the entity</param>
    /// <param name="includeRelatedEntities">Include related entities if true, else false.</param>
    /// <returns>A list of all entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/> </returns>
    ICollection<TEntity> FindAllById(IEnumerable<TId> ids, bool includeRelatedEntities = INCLUDE);

    /// <summary>
    /// Counts all entities of <typeparamref name="TEntity"/>
    /// </summary>
    /// <returns>Amount of entities of <typeparamref name="TEntity"/></returns>
    long CountAll();

    /// <summary>
    /// Counts all entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/>
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to count by.</param>
    /// <returns>Amount of entities of <typeparamref name="TEntity"/> with an id contained in <paramref name="ids"/> </returns>
    long CountAllById(IEnumerable<TId> ids);

    /// <summary>
    /// Check if entity exists by id.
    /// </summary>
    /// <param name="id">a <typeparam name="TId"/> to test if exists in the table.</param>
    /// <returns>true if the entity exists, else false.</returns>
    bool ExistsById(TId id);

    /// <summary>
    /// Check if there is an entity for each id in ids.
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to test if exists in the table.</param>
    /// <returns>true if all the ids have an entity, else false.</returns>
    bool ExistsAllById(IEnumerable<TId> ids);

    /// <summary>
    /// Check if at least one of the ids has an entity.
    /// </summary>
    /// <param name="ids">a list of <typeparam name="TId"/> to test if exists in the table.</param>
    /// <returns>true if at least one of the ids has an entity, else false.</returns>
    bool ExistsAnyById(IEnumerable<TId> ids);

    /// <summary>
    /// Saves and commits an entity to the DB.
    /// Use <see cref="Update"/> to update an existing entity. This method will throw an exception when saving an entity with an existing id.
    /// </summary>
    /// <param name="entity">The entity to be saved.</param>
    /// <exception cref="Exceptions.EntityWithSameIdExistsException">If the entity already exists.</exception>
    /// <returns> The saved entity.</returns>
    /// <seealso cref="Update"/>
    /// <seealso cref="SaveAll"/>
    TEntity? Save(TEntity entity);

    /// <summary>
    /// Saves and commits multiple entities to the DB.<br/>
    /// Use <see cref="UpdateAll"/> to update existing entities. This method will throw an exception when saving an entity with an existing id.
    /// </summary>
    /// <param name="entities">The entities to be saved.</param>
    /// <exception cref="Exceptions.EntityWithSameIdExistsException">If one or more entities already exist.</exception>
    /// <returns>The saved entities.</returns>
    /// <seealso cref="Save"/>
    /// <seealso cref="UpdateAll"/>
    ICollection<TEntity> SaveAll(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates and commits an entity to the DB.
    /// Use <see cref="Save"/> to save a new entity. This method will throw an exception when trying to update a non existing entity.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <exception cref="Exceptions.EntityIdDoesNotExistsException">if the entity is missing.</exception>
    /// <returns>The updated entity.</returns>
    /// <seealso cref="Save"/>
    /// <seealso cref="UpdateAll"/>
    TEntity? Update(TEntity entity);

    /// <summary>
    /// Updates and commits multiple entities to the DB.<br/>
    /// Use <see cref="SaveAll"/> to save new entities. This method will throw an exception when trying to update a non existing entity.
    /// </summary>
    /// <param name="entities">The entities to be updated.</param>
    /// <exception cref="Exceptions.EntityIdDoesNotExistsException">if the entity is missing.</exception>
    /// <returns>The updated entities.</returns>>
    /// <seealso cref="Update"/>
    /// <seealso cref="SaveAll"/>
    ICollection<TEntity> UpdateAll(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes multiple entities.
    /// </summary>
    /// <param name="entities">The entities to be deleted.</param>
    void DeleteAll(IEnumerable<TEntity> entities);

    /// <summary>
    /// Deletes an entity by id.
    /// </summary>
    /// <param name="id">The id of the entity to be deleted.</param>
    void DeleteById(TId id);

    /// <summary>
    /// Deletes multiple entities by id.
    /// </summary>
    /// <param name="ids">The ids of the entities to be deleted.</param>
    void DeleteAllById(IEnumerable<TId> ids);

    /// <summary>
    /// Deletes all the entities.
    /// </summary>
    void DeleteAll();
}