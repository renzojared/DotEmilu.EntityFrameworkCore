namespace DotEmilu.EntityFrameworkCore;

public interface IContextUser<out TUserKey>
    where TUserKey : struct
{
    TUserKey Id { get; }
}