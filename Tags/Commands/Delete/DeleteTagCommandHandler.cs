using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tags.Commands.Delete
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteTagCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Tags
                    .Include(x => x.ProductTags)
                    .Include(x => x.ContentTags)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Etiket bulunamadı.");

                // İlişkili verileri kontrol et
                if (entity.ProductTags.Any() || entity.ContentTags.Any())
                    return Result<bool>.Failure("Bu etiket kullanımda olduğu için silinemez.");

                _context.Tags.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Etiket silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}