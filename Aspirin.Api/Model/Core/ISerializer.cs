using Microsoft.Extensions.DependencyInjection;

namespace Aspirin.Api.Model.Core
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T value);
        T Deserialize<T>(byte[] bytes);
        T DeepCopy<T>(T other);
    }

    public class MessagePackSerializer : ISerializer
    {
        public byte[] Serialize<T>(T value)
        {
            return MessagePack.MessagePackSerializer.Serialize(value);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return MessagePack.MessagePackSerializer.Deserialize<T>(bytes);
        }

        public T DeepCopy<T>(T other)
        {
            if (other == null)
            {
                return default(T);
            }
            var bytes = Serialize(other);
            return Deserialize<T>(bytes);
        }
    }

    public static class SerializerExtensions
    {
        public static void AddMessagePackSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer, MessagePackSerializer>();
        }
    }
}
