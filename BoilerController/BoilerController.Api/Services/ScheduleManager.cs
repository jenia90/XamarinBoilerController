using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerController.Api.Services
{
    public class ScheduleManager
    {
        private static ScheduleManager manager;

        private ScheduleManager()
        {}

        public ScheduleManager GetInstance()
        {
            return manager ?? (manager = new ScheduleManager());
        }
    }
}
