using MessageAppAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace MessageAppAPI.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    DbSet<T> Table { get; } 
}