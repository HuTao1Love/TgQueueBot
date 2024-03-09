using FluentMigrator;
using Itmo.Dev.Platform.Postgres.Migrations;

namespace DataAccess.Migrations;

[Migration(1, "INIT")]
public class CreateDatabase : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
        => """
           create table if not exists users (
               id bigserial primary key,
               tgId bigint unique not null,
               name text not null,
               isAdmin bool not null
           );

           create table if not exists queues (
               id bigserial primary key,
               tgChatId bigint not null,
               tgMessageId bigint not null,
               name text not null,
               size int not null
           );

           create table if not exists users_queues (
               id bigserial primary key,
               userId bigint not null references users(id) on delete cascade,
               queueId bigint not null references queues(id) on delete cascade,
               position bigint not null
           )
           """;

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }
}