/*

This sketch is to be used with a Arduino with buttons connected to GND and a port. Arduino will report over serial which port is activated and how many times it has been activated since boot.

Ports 2-11 are pulled high with internal resistors, so no need for external pull up resistor.

Arduino reports a JSON string over serial. For example if port 2 is activated(connected to GND) for the 124th time, Arduino will send {"P":2,"A":124}

Note that if a port is activated/pulled low for more than one loop(approx 200ms), it will be regarded as another activation. 
To change this, each loop should register ports state, and only count/report when state changes back to low.

*/

int activations[10];
char buf[32];

void setup() {
  Serial.begin(9600);
  for(int i = 2; i < 12; i++)
  {
    pinMode(i, INPUT_PULLUP);
    activations[i - 2] = 0;
  }
  Serial.println("READY");
}

void loop() {
  for(int i = 2; i < 12; i++)
  {
    if(digitalRead(i) == LOW)
    {
      sprintf(buf, "{\"P\":%d,\"A\":%d}", i, ++activations[i - 2]);
      Serial.println(buf);
    }
  }
  delay(200);
}
