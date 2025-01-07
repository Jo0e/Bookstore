using DataAccess.DataConnection;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            AuthorRepository = new AuthorRepository(_context);
            BookRepository = new BookRepository(_context);
            CartItemRepository = new CartItemRepository(_context);
            CartRepository = new CartRepository(_context);
            CategoryRepository = new CategoryRepository(_context);  
            CommentRepository = new CommentRepository(_context);    
            ContactUsRepository = new ContactUsRepository(_context);
            MessageRepository = new MessageRepository(_context);
            PaymentRecordRepository = new PaymentRecordRepository(_context);
            WishlistRepository = new WishlistRepository(_context);
            
        }

        public IAuthorRepository AuthorRepository { get; private set; }

        public IBookRepository BookRepository { get; private set; }

        public ICartItemRepository CartItemRepository { get; private set; }

        public ICartRepository CartRepository { get; private set; }

        public ICategoryRepository CategoryRepository { get; private set; }

        public ICommentRepository CommentRepository { get; private set; }

        public IContactUsRepository ContactUsRepository { get; private set; }

        public IMessageRepository MessageRepository { get; private set; }

        public IPaymentRecordRepository PaymentRecordRepository { get; private set; }

        public IWishlistRepository WishlistRepository { get; private set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public Task CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose() => _context.Dispose();
    }
}
