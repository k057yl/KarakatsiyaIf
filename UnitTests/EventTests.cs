using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Features.Events.Commands.CreateEvent;
using Karakatsiya.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests
{
    public class EventTests
    {
        [Fact]
        public async Task Handle_ShouldThrowException_WhenOrganizerNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Karakatsiya_Test_Db")
                .Options;

            using var context = new AppDbContext(options);

            var sanitizerMock = new Mock<ISanitizerService>();

            var handler = new CreateEventHandler(context, sanitizerMock.Object);

            var command = new CreateEventCommand(
                Guid.NewGuid(),       // 1. UserId (генерируем случайный)
                "Тестовый ивент",     // 2. Title
                "Описание",           // 3. Description
                DateTime.UtcNow,      // 4. StartDate
                string.Empty,         // 5. Поле 5
                string.Empty,         // 6. Поле 6
                string.Empty,         // 7. Поле 7
                null,                 // 8. Поле 8
                null,                 // 9. Поле 9
                null,                 // 10. Поле 10
                null,                 // 11. Поле 11
                null,                 // 12. Поле 12
                null,                 // 13. Поле 13
                null,                 // 14. CategoryId (Guid?)
                new List<NestedCreateEventPhotoDto>(), // 15. Photos
                null                  // 16. PerformerIds
            );

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            Assert.Equal(AppConstants.Errors.ORGANIZER_NOT_FOUND, exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCategoryNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Karakatsiya_Test_Db")
                .Options;

            using var context = new AppDbContext(options);

            var sanitizerMock = new Mock<ISanitizerService>();

            var handler = new CreateEventHandler(context, sanitizerMock.Object);

            var command = new CreateEventCommand(
                Guid.NewGuid(),
                "Тестовый ивент",
                "Описание",
                DateTime.UtcNow,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,
                Guid.NewGuid(),
                new List<NestedCreateEventPhotoDto>(),
                null
            );

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            Assert.Equal(AppConstants.Errors.CATEGORY_NOT_EXIST, exception.Message);
        }
    }
}