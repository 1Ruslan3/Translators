using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class InterpreterStart
    {
        public void Start(string configFile, string programFile, int baseA, int baseI, int baseO)
        {
            InterpreterConfig.baseAssign = (baseA >= 2 && baseA <= 36) ? baseA : 10;
            InterpreterConfig.baseInput = (baseI >= 2 && baseI <= 36) ? baseI : 10;
            InterpreterConfig.baseOutput = (baseO >= 2 && baseO <= 36) ? baseO : 10;

            InterpreterConfig.LoadConfig(configFile);
            InterpreterConfig.SaveLastConfig(configFile);
            InterpreterExecute.ExecuteProgram(programFile);
        }
    }
}
