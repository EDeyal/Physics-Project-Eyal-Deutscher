using UnityEngine;

public class Metutelet : MonoBehaviour
{
    [SerializeField] Transform _pillarTop;
    [SerializeField] float _maxRadius;
    [SerializeField] bool _isRightDirection;
    [SerializeField] float _startingHight;
    [SerializeField] float _acceleration = 0;//starting with no acceleration
    [SerializeField] bool _isGoingUp = false;
    [SerializeField] float _speedFactor = 1;

    //use physics to make the ball drop down, you must keep the ball at a regular distance from the top part
    //when the ball is too far move it closer to the center, if it is too close move it to the current side
    //When the ball swings to the right side, it will go up 
    //make ball have acceleration and decceleration
    private void Start()
    {
        //get starting hight
        _startingHight = _pillarTop.position.y;
    }
    private void Update()
    {
        //Move Ball up or down;
        if (_isGoingUp)
        {
            //going up
            this.transform.position += new Vector3(0, _acceleration, 0);
        }
        else
        { 
            //going down
            this.transform.position -= new Vector3(0, _acceleration, 0);
        }

        //Adjust Acceleration
        if (_acceleration > 0)
        {
            //gain or lose acceleration
            if (_isGoingUp)
            {
                //deaccelerate when going up
                _acceleration -= Time.deltaTime * _speedFactor;
            }
            else
            {
                //accelerate when ggoing down
                _acceleration += Time.deltaTime * _speedFactor;
            }
        }
        else
        {
            //change dicrection
            if (_isRightDirection)
            {
                _isRightDirection = false;
            }
            else
            {
                _isRightDirection = true;
            }
        }
        //Adjust Ball position
        //Position 1 = Ball, Position 2 = Pillar Top
        //D = root((x1-x2)^2+(y1-y2)^2)
        //D^2 = (x1-x2)^2+(y1-y2)^2
        //D^2-(y1-y2)^2 = (x1-x2)^2
        //Root(D^2-(y1-y2)^2) = x1-x2
        //Root(D^2-(y1-y2)^2) + x2 = x1
        float distance = Vector3.Distance(this.transform.position, _pillarTop.position);
        if (distance > _maxRadius)
        {
            //too far, need to move closer
            if (_isRightDirection)
            {
                //move right
            }
            else
            {
                //move left
            }
        }
        else if (distance < _maxRadius)
        {
            //too close, need to move farther
            if (_isRightDirection)
            {
                //move left
            }
            else
            {
                //move right
            }
        }
        //Set Ball Direction with acceleration
    }
}
