using npascu_api_v1.Repository.Interface;

namespace npascu_api_v1.Services.Auth.Email.Implementation
{
    public class EmailValidationHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer? timer;
        private ILogger<EmailValidationHostedService> _logger;

        public EmailValidationHostedService(IServiceProvider serviceProvider, ILogger<EmailValidationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var checkInterval = TimeSpan.FromSeconds(120);
            timer = new Timer(state => CheckUnvalidatedEmails(), null, TimeSpan.Zero, checkInterval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private void CheckUnvalidatedEmails()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                _logger.LogInformation("Checking unvalidated emails");
                IEnumerable<string> unvalidatedEmails;
                var _authRepository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
                lock (_authRepository)
                {
                    unvalidatedEmails = _authRepository.GetUnvalidatedEmails();
                }

                if (unvalidatedEmails != null)
                {
                    if (!unvalidatedEmails.Any())
                    {
                        _logger.LogInformation("No unvalidated emails found");
                    }
                    else
                    {
                        foreach (var email in unvalidatedEmails)
                        {
                            if (TryDeleteUser(email))
                            {
                                _logger.LogInformation($"User with email {email} deleted.");
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while checking unvalidated emails.");
            }
        }

        private bool TryDeleteUser(string email)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var user = new Models.Entities.Auth.ApplicationUser();
                var _authRepository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();

                user = _authRepository.GetUser(email);

                if (user != null)
                {
                    var cutoffTime = user.CreatedAt.AddMinutes(5);
                    if (cutoffTime < DateTime.UtcNow)
                    {

                        _authRepository.DeleteUser(email);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while processing user with email {email}");
            }

            return false;
        }
    }
}
