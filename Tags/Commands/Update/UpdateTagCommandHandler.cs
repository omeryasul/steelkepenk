using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tags.Commands.Update
{
    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public UpdateTagCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<bool>> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _context.Tags
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                    return Result<bool>.Failure("Etiket bulunamadı.");

                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.Color = request.Color;
                entity.IsActive = request.IsActive;
                entity.UpdatedDate = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.UserEmail;

                // Slug güncelle
                entity.Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Tag", request.Id);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Etiket güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}