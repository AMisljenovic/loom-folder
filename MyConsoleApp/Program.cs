﻿﻿﻿﻿﻿using System.Globalization;
using MyConsoleApp.Cli;
using MyConsoleApp.Core;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

var runner = new ConsoleRunner(new MathSolver());
runner.Run();
