/* Encoder Library - Basic Example
 * http://www.pjrc.com/teensy/td_libs_Encoder.html
 *
 * This example code is in the public domain.
 */

#include <Encoder.h>

// Change these two numbers to the pins connected to your encoder.
//   Best Performance: both pins have interrupt capability
//   Good Performance: only the first pin has interrupt capability
//   Low Performance:  neither pin has interrupt capability
Encoder myEnc(3, 2);
//   avoid using pins with LEDs attached

const int button1Pin = 7;
const int button2Pin = 6;
int button1State = 0;
int button2State = 0;
int button1PrevState = 0;
int button2PrevState = 0;

int32_t oldPosition  = -999;

void setup() 
{
  Serial.begin(9600);
  
  pinMode(button1Pin, INPUT);
  pinMode(button2Pin, INPUT);
}

void loop() 
{
  
  int32_t newPosition = myEnc.read();
  
  if (newPosition != oldPosition) 
  {
    oldPosition = newPosition;
    Serial.println("e:"+newPosition);
  }
  
  button1State = digitalRead(button1Pin);
  button2State = digitalRead(button2Pin);
  
  if(button1State != button1PrevState)
  {
    button1PrevState = button1State;
    if (button1State == HIGH) 
    {     
      Serial.println("b1:0");
    } 
    else 
    {
      Serial.println("b1:1");
    }
  }
  
  if(button2State != button2PrevState)
  {
    button2PrevState = button2State;
    if (button2State == HIGH) 
    {     
      Serial.println("b2:0");
    } 
    else 
    {
      Serial.println("b2:1");
    }
  }
  
  delay(20);
}
