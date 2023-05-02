using Autofac.Extras.Moq;
using Moq;
using Order.Common.Models;
using Order.Core.Domain;
using Order.Data.Repositories;
using Order.Data.RepositoryWrappers;
using Order.Services.Mails;
using Shouldly;
using System.Linq.Expressions;

namespace Order.Test.Services
{
    [TestFixture]
    public class EmailMessageServiceTest
    {
        private AutoMock _autoMock;
        private Mock<IQueueMailRepositoryWrapper> _repositoryMock;
        private Mock<IEmailMessageRepository> _emailMessageRepositoryMock;
        private IEmailMessageService _emailMessageService;

        private const string _recipient = "test@example.com";
        private const string _subject = "Test email";
        private const string _body = "This is a test email.";

        [SetUp]
        public void SetUp()
        {
            _autoMock = AutoMock.GetLoose();
            _emailMessageService = _autoMock.Create<EmailMessageService>();
            _repositoryMock = _autoMock.Mock<IQueueMailRepositoryWrapper>();
            _emailMessageRepositoryMock = _autoMock.Mock<IEmailMessageRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _autoMock?.Dispose();
        }

        [Test]
        public async Task AddMessageAsync_WithValidInput_AddsMessageToRepository()
        {
            // Arrange
            var emailMessageModel = new AddEmailMessageModel(_recipient, _subject, _body);

            _repositoryMock.Setup(r => r.EmailMessageRepository)
                .Returns(_emailMessageRepositoryMock.Object);

            _repositoryMock.Setup(r => r.EmailMessageRepository.AddAsync(It.IsAny<EmailMessage>()))
                .Returns(Task.CompletedTask);

            // Act
            await _emailMessageService.AddMessageAsync(emailMessageModel);

            // Assert
            _repositoryMock.Verify(x => x.EmailMessageRepository.AddAsync(It.Is<EmailMessage>(m =>
                m.Recipient == _recipient &&
                m.Subject == _subject &&
                m.Body == _body &&
                m.CreatedAt <= DateTime.UtcNow &&
                m.LastUpdatedAt <= DateTime.UtcNow)), Times.Once);
        }

        [Test]
        public async Task GetAllUnsentMessagesAsync_ReturnsListOfEmailMessages_WhenUnsentMessagesExist()
        {
            // Arrange
            var emailMessages = GetEmailMessages();

            _repositoryMock.Setup(r => r.EmailMessageRepository)
                .Returns(_emailMessageRepositoryMock.Object);

            _repositoryMock.Setup(r => r.EmailMessageRepository.GetAsync(It.IsAny<Expression<Func<EmailMessage, bool>>>()))
                .ReturnsAsync(emailMessages)
                .Verifiable();

            // Act
            var result = await _emailMessageService.GetAllUnsentMessagesAsync();

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldBeOfType<List<EmailMessage>>(),
                result => result.All(em => !em.IsSent && em.SentCount < 4).ShouldBeTrue()
                );
        }

        [Test]
        public async Task GetAllUnsentMessagesAsync_ReturnsEmptyList_WhenNoUnsentMessagesExist()
        {
            // Arrange
            var emailMessages = new List<EmailMessage>();

            _repositoryMock.Setup(r => r.EmailMessageRepository)
               .Returns(_emailMessageRepositoryMock.Object);

            _repositoryMock.Setup(r => r.EmailMessageRepository.GetAsync(It.IsAny<Expression<Func<EmailMessage, bool>>>()))
                .ReturnsAsync(emailMessages)
                .Verifiable();

            // Act
            var result = await _emailMessageService.GetAllUnsentMessagesAsync();

            // Assert
            result.ShouldSatisfyAllConditions(
                result => _repositoryMock.Verify(),
                result => result.ShouldBeOfType<List<EmailMessage>>(),
                result => result.Count.ShouldBeEquivalentTo(0)
                );
        }

        [Test]
        public async Task UpdateMessageAsync_WithValidEmailMessage_CallsEditMethodOnce()
        {
            // Arrange
            var emailMessage = new EmailMessage { 
                Id = 1,
                Recipient = _recipient,
                Subject = _subject,
                Body = _body
            };

            _repositoryMock.Setup(r => r.EmailMessageRepository)
                .Returns(_emailMessageRepositoryMock.Object);

            _repositoryMock.Setup(r => r.EmailMessageRepository.Edit(emailMessage))
                .Verifiable();

            // Act
            await _emailMessageService.UpdateMessageAsync(emailMessage);

            // Assert
            _repositoryMock.Verify(r => r.EmailMessageRepository.Edit(emailMessage), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            _repositoryMock.Verify();
        }

        [Test]
        public async Task UpdateMessagesAsync_WithValidEmailMessage_CallsEditMethodOnce()
        {
            // Arrange
            var emailMessages = GetEmailMessages();

            _repositoryMock.Setup(r => r.EmailMessageRepository)
                .Returns(_emailMessageRepositoryMock.Object);

            _repositoryMock.Setup(r => r.EmailMessageRepository.EditRange(emailMessages))
                .Verifiable();

            // Act
            await _emailMessageService.UpdateMessagesAsync(emailMessages);

            // Assert
            _repositoryMock.Verify(r => r.EmailMessageRepository.EditRange(emailMessages), Times.Once);
            _repositoryMock.Verify(r => r.SaveAsync(), Times.Once);
            _repositoryMock.Verify();
        }

        private List<EmailMessage> GetEmailMessages()
        {
            return new List<EmailMessage>
            {
                new EmailMessage { Id = 1, Recipient = "test1@test.com", IsSent = false, SentCount = 0 },
                new EmailMessage { Id = 2, Recipient = "test2@test.com", IsSent = false, SentCount = 1 },
                new EmailMessage { Id = 3, Recipient = "test3@test.com", IsSent = false, SentCount = 2 },
            };
        }

    }
}
