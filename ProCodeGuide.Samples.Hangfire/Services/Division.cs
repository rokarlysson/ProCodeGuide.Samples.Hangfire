using System;

namespace ProCodeGuide.Samples.Hangfire.Services {
    public class Division : IDivision {
        public void DivisionRandom() {
            try {
                var number1 = new Random().Next(0, 5);
                var number2 = new Random().Next(0, 5);

                var result = number1 / number2;

            }
            catch (DivideByZeroException ex) {
                throw new Exception($"Divisão por zero {ex.Message}");
            }
        }
    }
}
