using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Contents.Commands.Delete
{
    public class DeleteContentCommandHandler : IRequestHandler<DeleteContentCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteContentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var content = await _context.Contents
                    .Include(x => x.ContentTags)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (content == null)
                {
                    return Result<bool>.Failure("İçerik bulunamadı.");
                }

                // Content-Tag ilişkilerini sil
                if (content.ContentTags.Any())
                {
                    _context.ContentTags.RemoveRange(content.ContentTags);
                }

                // Content'i sil
                _context.Contents.Remove(content);
                await _context.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"İçerik silinirken hata oluştu: {ex.Message}");
            }
        }
    }
}