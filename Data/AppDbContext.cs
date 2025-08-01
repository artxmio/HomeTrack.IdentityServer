﻿using HomeTrack.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.IdentityServer.Data;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(entity => 
            entity.ToTable(name: "Users"));

        builder.Entity<IdentityRole>(entity =>
            entity.ToTable(name: "Roles"));

        builder.Entity<IdentityUserRole<string>>(entity =>
            entity.ToTable(name: "UserRoles"));

        builder.Entity<IdentityUserClaim<string>>(entity =>
            entity.ToTable(name: "UserClaim"));

        builder.Entity<IdentityUserLogin<string>>(entity =>
            entity.ToTable("UserLogins"));

        builder.Entity<IdentityUserToken<string>>(entity =>
            entity.ToTable("UserTokens"));

        builder.Entity<IdentityRoleClaim<string>>(entity =>
            entity.ToTable("RoleClaims"));

        builder.ApplyConfiguration(new AppUserConfiguration());
    }
}