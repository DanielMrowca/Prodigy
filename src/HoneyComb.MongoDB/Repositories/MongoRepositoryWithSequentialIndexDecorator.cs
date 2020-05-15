using HoneyComb.CQRS.Queries;
using HoneyComb.Types;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HoneyComb.MongoDB.Repositories
{
    public class MongoRepositoryWithSequentialIndexDecorator<TEntity, TKey> : IMongoRepository<TEntity, TKey>
        where TEntity : ISequentialIndex<TKey>
    {
        private readonly ISequentialIndexProvider<TEntity, TKey> _indexProvider;
        private readonly IMongoRepository<TEntity, TKey> _decoratedRepository;
        public IMongoCollection<TEntity> Collection => _decoratedRepository.Collection;

        public MongoRepositoryWithSequentialIndexDecorator(IMongoRepository<TEntity, TKey> mongoRepository, ISequentialIndexProvider<TEntity, TKey> indexProvider)
        {
            _indexProvider = indexProvider;
            _decoratedRepository = mongoRepository;
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await _decoratedRepository.FindAsync(predicate);

        public async Task<TEntity> GetAsync(TKey id)
            => await _decoratedRepository.GetAsync(id);

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await _decoratedRepository.GetAsync(predicate);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => await _decoratedRepository.ExistsAsync(predicate);

        public async Task<PagedResult<TEntity>> BrowseAsync<TQuery>(Expression<Func<TEntity,
            bool>> predicate, TQuery query) where TQuery : IPagedQuery
            => await _decoratedRepository.BrowseAsync(predicate, query);


        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.Index = _indexProvider.GetNextIndex();
            return await _decoratedRepository.AddAsync(entity);
        }


        public async Task UpdateAsync(TEntity entity)
            => await _decoratedRepository.UpdateAsync(entity);

        public async Task DeleteAsync(TKey id)
            => await _decoratedRepository.DeleteAsync(id);

        public async Task<IEnumerable<TEntity>> AddMultipleAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                entity.Index = _indexProvider.GetNextIndex();

            return await _decoratedRepository.AddMultipleAsync(entities);
        }

    }
}
