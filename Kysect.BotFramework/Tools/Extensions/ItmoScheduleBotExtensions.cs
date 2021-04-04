﻿using System.Threading.Tasks;

namespace Kysect.BotFramework.Tools.Extensions
{
    //TODO: rename?
    public static class ItmoScheduleBotExtensions
    {
        public static void WaitSafe(this Task task)
        {
            Task.WaitAny(task);
        }

        public static void WaitSafe<T>(this Task<T> task)
        {
            Task.WaitAny(task);
        }
    }
}