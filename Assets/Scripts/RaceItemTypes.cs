using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**/
/*
    This class is an abstract base class for items that can be used in the game. It defines a common 
    interface for all race items, requiring the implementation of a Use method, which specifies how each item 
    affects the car or the race when used. 
*/
/**/
public abstract class RaceItem
{
    public abstract void Use(CarController carController);
}

/**/
/*
    The SpeedBoost class extends RaceItem and represents a speed boost power-up in the game. When used, 
    it temporarily increases the car's speed and plays a particle effect. 
*/
/**/
public class SpeedBoost : RaceItem
{
    /**/
    /*
    SpeedBoost::Use(CarController carController) SpeedBoost::Use(CarController carController)

    NAME
        SpeedBoost::Use - Implements the use action for a SpeedBoost item.

    SYNOPSIS
        public override void Use(CarController carController);
            carController   --> The car controller on which the SpeedBoost will be applied.

    DESCRIPTION
        When a SpeedBoost item is used, this method is invoked to apply a speed boost effect to the car. 

    RETURNS
        Nothing.
    */
    /**/
    public override void Use(CarController carController)
    {
        carController.BoostSpeed(this);
        carController.speedBoostParticles.Play();
    }

    /**/
    /*
    SpeedBoost::EndBoost(CarController carController) SpeedBoost::EndBoost(CarController carController)

    NAME
        SpeedBoost::EndBoost - Ends the speed boost effect on the car.

    SYNOPSIS
        public void EndBoost(CarController carController);
            carController   --> The car controller from which the speed boost will be removed.

    DESCRIPTION
        This method is called to end the speed boost effect on the car. It resets any modifications
        made to the car's speed and stops related effects.

    RETURNS
        Nothing.
    */
    /**/
    public void EndBoost(CarController carController)
    {
        carController.maxSpeed = carController.originalMaxSpeed;

        if (carController.speedBoostParticles != null)
        {
            carController.speedBoostParticles.Stop();
        }
    }
}

/**/
/*
    The MudPuddle class extends RaceItem and represents an item that creates a mud puddle on the track. 
    This puddle affects the speed of cars that pass through it. 
*/
/**/
public class MudPuddle : RaceItem
{
    public float duration = 5f;

    /**/
    /*
    MudPuddle::Use(CarController carController) MudPuddle::Use(CarController carController)

    NAME
        MudPuddle::Use - Implements the use action for a MudPuddle item.

    SYNOPSIS
        public override void Use(CarController carController);
            carController   --> The car controller on which the MudPuddle will be applied.

    DESCRIPTION
        When a MudPuddle item is used, this method is invoked to create a mud puddle effect on the race track. 

    RETURNS
        Nothing.
    */
    /**/
    public override void Use(CarController carController)
    {
        carController.DropMudPuddle(duration);
    }
}
