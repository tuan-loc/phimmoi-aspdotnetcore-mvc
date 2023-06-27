﻿using Microsoft.AspNetCore.Identity;

namespace PhimMoi.Application.Models
{
    public class LoginResult : Result
    {
        public bool IsLockedOut { get; set; }

        public new static LoginResult OK() => new LoginResult { Success = true };

        public new static LoginResult Error(params string[] errors)
        {
            return new LoginResult
            {
                Success = false,
                Errors = errors.ToList()
            };
        }

        public static LoginResult FromSignResult(SignInResult signInResult)
        {
            if (signInResult.Succeeded) return OK();

            if (signInResult.IsLockedOut) return new LoginResult { Success = false, IsLockedOut = true };

            return new LoginResult { Success = false };
        }
    }
}