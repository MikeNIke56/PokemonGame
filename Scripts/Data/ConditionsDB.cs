using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static Dictionary<ConditionsID, Conditions> Conditions { get; set; } = new Dictionary<ConditionsID, Conditions>()
    {
        {
            ConditionsID.psn,
            new Conditions()
            {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = (PokemonInfo pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 8);
                    BattleSystem.i.curHP.text = pokemon.HP.ToString();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took poison damage");
                }
            }

        },
        {
            ConditionsID.brn,
            new Conditions()
            {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = (PokemonInfo pokemon) =>
                {
                    pokemon.DecreaseHP(pokemon.MaxHp / 16);
                    BattleSystem.i.curHP.text = pokemon.HP.ToString();
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took burn damage");
                }
            }

        },
         {
            ConditionsID.par,
            new Conditions()
            {
                Name = "Paralyzed",
                StartMessage = "has been paralyzed",
                OnBeforeTurn = (PokemonInfo pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed and can't move");
                        return true;
                    }
                   return false;
                }
            }

         },
         {
            ConditionsID.frz,
            new Conditions()
            {
                Name = "Frozen",
                StartMessage = "has been frozen",
                OnBeforeTurn = (PokemonInfo pokemon) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thawed out");
                        return false;
                    }
                    else
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is frozen solid");
                        return true;
                    }
                }
            }

         },
         {
            ConditionsID.slp,
            new Conditions()
            {
                Name = "Asleep",
                StartMessage = "has fallen asleep",
                OnStart = (PokemonInfo pokemon) =>
                {
                    //sleep for 1-3 turns
                    pokemon.StatusTime = Random.Range(1, 4);                 
                },
                OnBeforeTurn = (PokemonInfo pokemon) =>
                {
                    if (pokemon.StatusTime <= 0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up");
                        return false;
                    }
                    else
                    {
                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping");
                        return true;
                    }
                }
            }

         },

         //volatile status conditions

         {
            ConditionsID.confusion,
            new Conditions()
            {
                Name = "Confusion",
                StartMessage = "is confused",
                OnStart = (PokemonInfo pokemon) =>
                {
                    //confused for 1-4 turns
                    pokemon.VolatileStatusTime = Random.Range(1, 5);
                },
                OnBeforeTurn = (PokemonInfo pokemon) =>
                {
                    if (pokemon.VolatileStatusTime <= 0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} broke out of confusion");
                        return false;
                    }
                    pokemon.VolatileStatusTime--;

                    //50% chance to do a move
                    if (Random.Range(1, 3) == 1)
                        return false;
                    else
                    {
                         //hurt by confusion         
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confused");
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        BattleSystem.i.curHP.text = pokemon.HP.ToString();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself in confusion");
                        return true;
                    }                
                }
            }

         },
         {
            ConditionsID.sunny,
            new Conditions()
            {
                Name = "Harsh Sunlight",
                StartMessage = "The weather has changed to harsh sunlight",
                EffectMessage = "The sunlight is harsh",
                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Fire)
                    {
                       return 1.5f;
                    }
                    else if(move.Base.Type == PokemonType.Water)
                    {
                        return .5f;
                    }

                    return 1f;
                }
            }

         },
         {
            ConditionsID.rain,
            new Conditions()
            {
                Name = "Heavy Rain",
                StartMessage = "The weather has changed to heavy rain",
                EffectMessage = "The rain continues to fall",
                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Water)
                    {
                       return 1.5f;
                    }
                    else if(move.Base.Type == PokemonType.Fire)
                    {
                        return .5f;
                    }
                    /*else if(move.Base.Type == PokemonType.Electric)
                    {
                        move.Base.alwaysHits = true;
                    }*/

                    return 1f;
                }
            }

         },
         {
            ConditionsID.sandstorm,
            new Conditions()
            {
                Name = "Sandstorm",
                StartMessage = "The weather has changed to a sandstorm",
                EffectMessage = "The sandstorm is raging",
                OnWeather = (PokemonInfo pokemon) =>
                {
                    if (pokemon.Base.Type1 == PokemonType.Ground || pokemon.Base.Type1 == PokemonType.Rock ||
                        pokemon.Base.Type1 == PokemonType.Steel || pokemon.Base.Type2 == PokemonType.Ground ||
                        pokemon.Base.Type2 == PokemonType.Rock ||pokemon.Base.Type2 == PokemonType.Steel)
                    {
                        return;
                    }
                    else
                    {
                        pokemon.DecreaseHP(Mathf.RoundToInt((float)pokemon.MaxHp / 16));
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} has been buffeted by the sandstorm");
                    } 
                }
            }

         },
         {
            ConditionsID.veil,
            new Conditions()
            {
                Name = "Veil",
                StartMessage = "The weather has changed to a Winter Veil",
                EffectMessage = "The veil is deep",
                OnWeather = (PokemonInfo pokemon) =>
                {
                    if (pokemon.Base.Type1 == PokemonType.Ice || pokemon.Base.Type2 == PokemonType.Ice)
                    {
                        return;
                    }
                    else
                    {
                        pokemon.DecreaseHP(Mathf.RoundToInt((float)pokemon.MaxHp / 16));
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} has been buffeted by the hail");
                    }
                }
            }

         },
         {
            ConditionsID.desLand,
            new Conditions()
            {
                Name = "Desolate Land",
                StartMessage = "The Sun lets down a scorching heat",
                EffectMessage = "The heat continues to batter down",
                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Fire)
                    {
                       return 2f;
                    }
                    else if(move.Base.Type == PokemonType.Water)
                    {
                        return 0f;
                    }

                    return 1f;
                }
            }

         },
         {
            ConditionsID.primSea,
            new Conditions()
            {
                Name = "Primordial Sea",
                StartMessage = "A fierce rainstorm has appeared",
                EffectMessage = "The rain continues to pour down ferociously",
                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Water)
                    {
                       return 2f;
                    }
                    else if(move.Base.Type == PokemonType.Fire)
                    {
                        return 0f;
                    }
                    /*else if(move.Base.Type == PokemonType.Electric)
                    {
                        move.Base.alwaysHits = true;
                    }*/

                    return 1f;
                }
            }

         },
         {
            ConditionsID.deltaStream,
            new Conditions()
            {
                Name = "Delta Stream",
                StartMessage = "A strong air current envelops the area",
                EffectMessage = "The currents remain strong",
                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Flying)
                    {
                       return 2f;
                    }
                    else if(target.Base.Type1 == PokemonType.Flying || target.Base.Type2 == PokemonType.Flying)
                    {
                        return .5f;
                    }

                    return 1f;
                }
            }

         },
         {
            ConditionsID.creatorsWill,
            new Conditions()
            {
                Name = "Creator's Will",
                StartMessage = "The air gets heavy, an immense pressure floods into the area",
                EffectMessage = "You can't shake this feeling of helplessness",
                OnWeather = (PokemonInfo pokemon) =>
                {
                    if (pokemon.Base.Name != "Arceus")
                    {
                        pokemon.DecreaseHP(Mathf.RoundToInt((float)pokemon.MaxHp / 5));
                        pokemon.StatusChanges.Enqueue($"Fragments of the universe tear into {pokemon.Base.Name}!");
                    }
                },

                OnDamageModify = (PokemonInfo source, PokemonInfo target, Move move) =>
                {
                    if (move.Base.Type == PokemonType.Normal)
                    {
                       return 2f;
                    }
                    else if(target.Base.Name == "Arceus")
                    {
                        return .5f;
                    }

                    return 1f;
                }
            }

         }  
    };  
    public static float GetStatusBonus(Conditions conditions)
    {
        if (conditions == null)
            return 1f;
        else if (conditions.Id == ConditionsID.confusion)
            return 1.2f;
        else if (conditions.Id == ConditionsID.brn || conditions.Id == ConditionsID.par || conditions.Id == ConditionsID.psn)
            return 1.5f;
        else if (conditions.Id == ConditionsID.slp || conditions.Id == ConditionsID.frz)
            return 2f;

        return 1f;
    }
}

public enum ConditionsID
{
   none, psn, brn, slp, par, frz, 
   confusion,
   sunny, rain, sandstorm, veil, desLand, deltaStream, primSea, creatorsWill
}