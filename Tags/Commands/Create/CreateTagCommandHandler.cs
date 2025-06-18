using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;

namespace Application.Features.Tags.Commands.Create
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<int>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly ICurrentUserService _currentUser;

        public CreateTagCommandHandler(
            IApplicationDbContext context,
            ISlugService slugService,
            ICurrentUserService currentUser)
        {
            _context = context;
            _slugService = slugService;
            _currentUser = currentUser;
        }

        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new Tag
                {
                    Name = request.Name,
                    Slug = await _slugService.GenerateUniqueSlugAsync(request.Name, "Tag"),
                    Description = request.Description,
                    Color = request.Color,
                    IsActive = request.IsActive,
                    CreatedBy = _currentUser.UserEmail
                };

                _context.Tags.Add(entity);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Etiket oluşturulurken hata oluştu: {ex.Message}");
            }
        }
    }
}