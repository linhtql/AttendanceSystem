﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attendance.Client
{
    public interface IAsyncClient
    {
        Task RunAsync();
    }
}
