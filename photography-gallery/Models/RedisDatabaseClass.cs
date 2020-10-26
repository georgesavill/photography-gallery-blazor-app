using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace photography_gallery.Models
{
    public static class RedisDatabaseClass
    {
        public static IDatabase redisDatabase;

        public static IDatabase RedisDatabase
        {
            get
            {
                return redisDatabase;
            }
            set
            {
                redisDatabase = value;
            }
        }
    }
}