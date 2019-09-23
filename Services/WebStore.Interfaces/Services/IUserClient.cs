using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using WebStore.Domain.Entities;

namespace WebStore.Interfaces.Services
{
    public interface IUserClient : IUserRoleStore<User>,
                                   IUserPasswordStore<User>,
                                   IUserEmailStore<User>,
                                   IUserPhoneNumberStore<User>,
                                   IUserClaimStore<User>,
                                   IUserTwoFactorStore<User>,
                                   IUserLoginStore<User>,
                                   IUserLockoutStore<User>
    {
    }
}
