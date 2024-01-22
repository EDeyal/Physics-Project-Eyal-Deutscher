using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Metutelet : MonoBehaviour
{
    [SerializeField] Transform _pillarTop;
    [SerializeField] float _maxRadius;
    [SerializeField] float _acceleration = 0;//starting with no acceleration
    [SerializeField] bool _isGoingUp = false;
    [SerializeField] bool _isOnRightSide;
    [SerializeField] float _speedFactor = 1;
    [SerializeField] float _gravity = 9.81f;
    [SerializeField] float _moveToSidemount = 0.001f;
    [Header("Do Not Edit")]
    [SerializeField] float _startingHight;
    [SerializeField] float _maxHight;
    //use physics to make the ball drop down, you must keep the ball at a regular distance from the top part
    //when the ball is too far move it closer to the center, if it is too close move it to the current side
    //When the ball swings to the right side, it will go right if it goes up it will go up
    //make ball have acceleration and decceleration
    private void Start()
    {
        //get starting hight
        _startingHight = this.transform.position.y;
        //we start with falling down
        _isGoingUp=false;

        //check if on the right side or the left side & middle sided
        CheckIsOnRightSide();
    }
    private bool CheckIsOnRightSide()
    {
        if (this.transform.position.x > _pillarTop.transform.position.x)
        {
            _isOnRightSide = true;
        }
        else
        {
            _isOnRightSide = false;
        }
        return _isOnRightSide;
    }
    private void MoveBallUpDown()
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
    }
    private void MoveBallLeftRight()
    {
        //move the ball left or right
        if ((_isOnRightSide & _isGoingUp) || (!_isOnRightSide & !_isGoingUp))
        {
            //going right
            this.transform.position += new Vector3(+_moveToSidemount, 0, 0);
        }
        else
        {
            //going left
            this.transform.position -= new Vector3(-_moveToSidemount, 0, 0);
        }
    }
    private void ApplyAcceleration()
    {
        //gain or lose acceleration
        if (_isGoingUp)
        {
            //deaccelerate when going up
            _acceleration -= Time.fixedDeltaTime * _speedFactor;
        }
        else
        {
            //accelerate when ggoing down
            _acceleration += Time.fixedDeltaTime * _speedFactor;
        }
    }
    private void FixedUpdate()
    {
        MoveBallLeftRight();

        MoveBallUpDown();

        //Adjust Acceleration
        if (_acceleration < 0)
        {
            //change dicrection
            _isGoingUp = false;
            _acceleration = 0;
            //if(_maxHight < this.transform.position.y)
            //{
                _maxHight = this.transform.position.y;
            //}
        }
        ApplyAcceleration();

        //set direction ball is going to
        if (_isGoingUp)
        {
            ////if is over the top, stop going up
            //if (this.transform.position.y > _pillarTop.position.y)
        }
        else
        {
            //if lower than lowest point, go up and set position to lowest point.
            if (this.transform.position.y < (_pillarTop.position.y - _maxRadius))
            {
                _isGoingUp = true;
                _isOnRightSide = !_isOnRightSide;
                this.transform.position = new Vector3(transform.position.x, (_pillarTop.position.y - _maxRadius), transform.position.z);
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
        //if (distance > _maxRadius)
        //{
            //too far, need to move closer
            float x1Calculation;
            float d = _maxRadius * _maxRadius;//D^2
            float yValue = this.transform.position.y - _pillarTop.transform.position.y;//y1-y2
            yValue = yValue * yValue;//(y1-y2)^2
            x1Calculation = MathF.Sqrt(d - yValue);//Root(D^2-(y1-y2)^2)
            x1Calculation = x1Calculation + _pillarTop.transform.position.x;//Root(D^2-(y1-y2)^2) + x2

            if (_isOnRightSide && _isGoingUp)
            {
                //not nessery but helps to think
                //x1Calculation = Mathf.Abs(x1Calculation);
            }
            else if (!_isOnRightSide && _isGoingUp)
            {
                x1Calculation = -x1Calculation;
            }
            else if (_isOnRightSide && !_isGoingUp)
            {
                //not nessery but helps to think
                //x1Calculation = Mathf.Abs(x1Calculation);
            }
            else if (!_isOnRightSide && !_isGoingUp)
            {
                x1Calculation = -x1Calculation;
            }
            //set X
            this.transform.position = new Vector3(x1Calculation, transform.position.y, transform.position.z);
        //}
        //else if (distance < _maxRadius)
        //{
        //    //too close, need to move farther
        //    float x1Calculation;
        //    float d = _maxRadius * _maxRadius;//D^2
        //    float yValue = this.transform.position.y - _pillarTop.transform.position.y;//y1-y2
        //    yValue = yValue * yValue;//(y1-y2)^2
        //    x1Calculation = MathF.Sqrt(d - yValue);//Root(D^2-(y1-y2)^2)
        //    x1Calculation = x1Calculation + _pillarTop.transform.position.x;//Root(D^2-(y1-y2)^2) + x2
        //                                                                    //set X
        //    this.transform.position = new Vector3(x1Calculation, transform.position.y, transform.position.z);
        //}
        //Set Ball Direction with acceleration
        if (_isGoingUp)
        {
            _acceleration -= _gravity;
        }
        else
        {
            _acceleration += _gravity;
        }
        if (_acceleration <= 0)
        {
            //change direction
            if (_isGoingUp)
            {
                _isGoingUp = false;
            }
            else
            {
                _isGoingUp = true;
            }
        }

    }
}
