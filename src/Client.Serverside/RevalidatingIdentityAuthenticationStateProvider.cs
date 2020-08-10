using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BDMT.Client.Serverside
{
    public class RevalidatingIdentityAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider
        where TUser : class
    {
        private readonly IdentityOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;

        public RevalidatingIdentityAuthenticationStateProvider(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, IOptions<IdentityOptions> optionsAccessor)
            : base(loggerFactory)
        {
            this._scopeFactory = scopeFactory;
            this._options = optionsAccessor.Value;
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = this._scopeFactory.CreateScope();
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await this.ValidateSecurityStampAsync(userManager, authenticationState.User);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                return false;
            }

            if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }

            var principalStamp = principal.FindFirstValue(this._options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }

    /// <summary>
    ///     A base class for <see cref="AuthenticationStateProvider" /> services that receive an
    ///     authentication state from the host environment, and revalidate it at regular intervals.
    /// </summary>
    public abstract class RevalidatingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
    {
        private readonly ILogger _logger;
        private CancellationTokenSource _loopCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        ///     Constructs an instance of <see cref="RevalidatingServerAuthenticationStateProvider" />.
        /// </summary>
        /// <param name="loggerFactory">A logger factory.</param>
        public RevalidatingServerAuthenticationStateProvider(ILoggerFactory loggerFactory)
        {
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this._logger = loggerFactory.CreateLogger<RevalidatingServerAuthenticationStateProvider>();

            // Whenever we receive notification of a new authentication state, cancel any
            // existing revalidation loop and start a new one
            this.AuthenticationStateChanged += authenticationStateTask =>
            {
                this._loopCancellationTokenSource?.Cancel();
                this._loopCancellationTokenSource = new CancellationTokenSource();
                _ = this.RevalidationLoop(authenticationStateTask, this._loopCancellationTokenSource.Token);
            };
        }

        /// <summary>
        ///     Gets the interval between revalidation attempts.
        /// </summary>
        protected abstract TimeSpan RevalidationInterval { get; }

        void IDisposable.Dispose()
        {
            this._loopCancellationTokenSource?.Cancel();
            this.Dispose(true);
        }

        /// <summary>
        ///     Determines whether the authentication state is still valid.
        /// </summary>
        /// <param name="authenticationState">The current <see cref="AuthenticationState" />.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while performing the operation.</param>
        /// <returns>
        ///     A <see cref="Task" /> that resolves as true if the <paramref name="authenticationState" /> is still valid, or
        ///     false if it is not.
        /// </returns>
        protected abstract Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken);

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
        }

        private async Task RevalidationLoop(Task<AuthenticationState> authenticationStateTask, CancellationToken cancellationToken)
        {
            try
            {
                var authenticationState = await authenticationStateTask;
                if (authenticationState.User.Identity?.IsAuthenticated ?? false)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        bool isValid;

                        try
                        {
                            await Task.Delay(this.RevalidationInterval, cancellationToken);
                            isValid = await this.ValidateAuthenticationStateAsync(authenticationState, cancellationToken);
                        }
                        catch (TaskCanceledException tce)
                        {
                            // If it was our cancellation token, then this revalidation loop gracefully completes
                            // Otherwise, treat it like any other failure
                            if (tce.CancellationToken == cancellationToken)
                            {
                                break;
                            }

                            throw;
                        }

                        if (!isValid)
                        {
                            this.ForceSignOut();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while revalidating authentication state");
                this.ForceSignOut();
            }
        }

        private void ForceSignOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var anonymousState = new AuthenticationState(anonymousUser);
            this.SetAuthenticationState(Task.FromResult(anonymousState));
        }
    }

    /// <summary>
    ///     An <see cref="AuthenticationStateProvider" /> intended for use in server-side Blazor.
    /// </summary>
    public class ServerAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        private Task<AuthenticationState>? _authenticationStateTask;

        /// <inheritdoc />
        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            this._authenticationStateTask = authenticationStateTask ?? throw new ArgumentNullException(nameof(authenticationStateTask));
            this.NotifyAuthenticationStateChanged(this._authenticationStateTask);
        }

        /// <inheritdoc />
        public override Task<AuthenticationState> GetAuthenticationStateAsync() => this._authenticationStateTask ?? throw new InvalidOperationException($"{nameof(this.GetAuthenticationStateAsync)} was called before {nameof(this.SetAuthenticationState)}.");
    }
}