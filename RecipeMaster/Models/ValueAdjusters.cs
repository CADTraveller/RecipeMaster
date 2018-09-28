using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RecipeMaster.Models
{
    public static class ValueAdjusters
    {
        public static void AdjustIngredientPercentages(object senderObj, double newPercent, ObservableCollection<Ingredient> ingredients)
        {
            Ingredient sender = senderObj as Ingredient;
            double oldPercent = sender.GetExactPercent();
            List<Ingredient> freeIngredients = ingredients.Where(i => !i.RatioLocked).ToList();
            double freePercentagesTotal = freeIngredients.Sum(i => i.GetExactPercent());
            double oldRemainder = freePercentagesTotal - oldPercent;
            double newRemainder = freePercentagesTotal - newPercent;
            
            foreach (Ingredient item in freeIngredients)
            { 
                if (item == sender) continue;

                double adjustedPercent = (item.GetExactPercent() / oldRemainder) * newRemainder;
                item.AdjustPercent(adjustedPercent);
            }
            
        }

        public static void AdjustItemWeights(Ingredient sender, double newWeight,  ObservableCollection<Ingredient> items )
        {
            double oldWeight = sender.GetExactWeight();
            double weightAdjustRatio = newWeight / oldWeight;
            

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == sender) continue;
                Ingredient ingredient = items[i];
                double updatedWeight = ingredient.GetExactWeight() * weightAdjustRatio;
                ingredient.AdjustWeight(updatedWeight);
                
            }
            sender.AdjustWeight(newWeight);
        }

        public static void AdjustIngredientPercentages(object senderObj, double newPercent, List<Ingredient> ingredients)
        {
            Ingredient sender = senderObj as Ingredient;
            double oldPercent = sender.GetExactPercent();
            double oldRemainder = 100 - oldPercent;
            double newRemainder = 100 - newPercent;

            for (int i = 0; i < ingredients.Count; i++)
            {
                Ingredient item = ingredients[i];
                if (item == sender) continue;

                double adjustedPercent = (item.GetExactPercent() / oldRemainder) * newRemainder;
                item.AdjustPercent(adjustedPercent);
            }

        }

        public static void AdjustItemWeights(object senderObj, double newWeight, List<Ingredient> items)
        {
            Ingredient sender = senderObj as Ingredient;
            double oldWeight = sender.GetExactWeight();
            double weightAdjustRatio = newWeight / oldWeight;


            for (int i = 0; i < items.Count; i++)
            {
                Ingredient ingredient = items[i];
                if (ingredient == sender) continue;
                //if (i == items.Count - 1)
                //{
                //    ingredient.Weight = weightRemaining;
                //    break;
                //}
                double updatedWeight = ingredient.GetExactWeight() * weightAdjustRatio;

                ingredient.AdjustWeight(updatedWeight);
            }

        }

        public static void SetPercentageFromWeight(ObservableCollection<Ingredient> items)
        {
            //__when ratios turned back on, loop through newly entered weights to set values
            double totalWeight = items.Sum(i => i.GetExactWeight());
            foreach (Ingredient ingredient in items)
            {
                ingredient.AdjustPercent((ingredient.GetExactWeight() / totalWeight) * 100);
                //SetChildWeightsFromPercentage(ingredient);
            }
        }

        public static void SetChildWeightsFromPercentage( Ingredient parentIngredient)
        {
            var children = parentIngredient.Ingredients;
            if (children == null || children.Count == 0) return;
            double totalWeight = parentIngredient.GetExactWeight();

            foreach (var child in children) child.AdjustWeight(child.GetExactPercent() * totalWeight);

        }

    }
}
