using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Data.Entities.Audience;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Karakatsiya.Features.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, (bool Success, Guid? CommentId, string MessageKey)>
    {
        private readonly AppDbContext _db;

        public CreateCommentCommandHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(bool Success, Guid? CommentId, string MessageKey)> Handle(CreateCommentCommand request, CancellationToken ct)
        {
            var eventExists = await _db.Events.AnyAsync(e => e.Id == request.EventId, ct);
            if (!eventExists)
            {
                return (false, null, AppConstants.Errors.EVENT_NOT_FOUND);
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId,
                UserId = request.UserId,
                Text = request.Text,
                ShowInstagram = request.ShowInstagram,
                ShowTelegram = request.ShowTelegram
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync(ct);

            return (true, comment.Id, AppConstants.Success.REQUEST_APPROVED);
        }
    }
}
