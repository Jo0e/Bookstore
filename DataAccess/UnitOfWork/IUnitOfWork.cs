using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthorRepository AuthorRepository { get; }
        IBookRepository BookRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ICartRepository CartRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICommentRepository CommentRepository { get; }
        IContactUsRepository ContactUsRepository { get; }
        IMessageRepository MessageRepository { get; }
        IPaymentRecordRepository PaymentRecordRepository { get; }
        IWishlistRepository WishlistRepository { get; }
        IUserRepository UserRepository { get; }
        int Complete();
        Task CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
