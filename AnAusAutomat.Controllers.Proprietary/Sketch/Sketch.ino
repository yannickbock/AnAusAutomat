enum Method
{
  Hello = 0,
  Ping = 1,
  TurnOff = 2,
  TurnOn = 3
};

typedef struct
{
  int Id;
  int OutputPin;
  int InputPin;
} Socket;

Socket Sockets[4];
String DeviceName;
String NO_NOT_DELETE; // else you can´t concat strings... very strange...
int NumberOfSockets;
long SessionId;

void setup()
{
  DeviceName = "Sam";

  Sockets[0].Id = 1;
  Sockets[0].InputPin = 5;
  Sockets[0].OutputPin = 6;

  Sockets[1].Id = 2;
  Sockets[1].InputPin = 7;
  Sockets[1].OutputPin = 8;

  Sockets[2].Id = 3;
  Sockets[2].InputPin = 9;
  Sockets[2].OutputPin = 10;

  Sockets[3].Id = 4;
  Sockets[3].InputPin = 11;
  Sockets[3].OutputPin = 12;

  setupSockets();
  generateSessionId();

  Serial.begin(38400);

  delay(25);
}

void generateSessionId()
{
  randomSeed(analogRead(0));
  SessionId = random(1000, 15000);
}

void setupSockets()
{
  NumberOfSockets = sizeof(Sockets) / sizeof(*Sockets);
  
  for (int i = 0; i < NumberOfSockets; i++)
  {
    pinMode(Sockets[i].OutputPin, OUTPUT);
    pinMode(Sockets[i].InputPin, INPUT);
  }
}

void loop()
{
}

void serialEvent()
{
  int value = Serial.read();
  int method = (value & 3);
  int socketId  = (value - (int)method) >> 2;

  switch (method)
  {
    case Hello:
      printHello();
      break;
    case Ping:
      Serial.print(String(SessionId));
      break;
    case TurnOff:
      Serial.print(switchSocket(socketId, false));
      break;
    case TurnOn:
      Serial.print(switchSocket(socketId, true));
      break;
  }
}
void printHello()
{
  String message = "AnAusAutomat|" + DeviceName + "|{";

  for (int i = 0; i < NumberOfSockets; i++)
  {
    message += String(Sockets[i].Id) + ";";
  }
  message = message.substring(0, message.length() - 1);
  message += "}";
  
  Serial.print(message);
}

bool switchSocket(int id, bool powerOn)
{
  Socket socket = getSocket(id);
  bool isActiveLow = isNegativeLogic(socket.OutputPin, socket.InputPin);

  // powerOn | isActiveLow | pinResult
  // true    | true            | LOW
  // false   | true            | HIGH
  // true    | false           | HIGH
  // false   | false           | LOW
  // => powerOn == isActiveLow = LOW = 0
  // => powerOn != isActiveLow = HIGH = 1
  bool switchToHigh = powerOn != isActiveLow;

  digitalWrite(socket.OutputPin, switchToHigh);

  return digitalReadOutputPin(socket.OutputPin) == switchToHigh;
}

Socket getSocket(int id)
{
  for (int i = 0; i < NumberOfSockets; i++)
  {
    if (Sockets[i].Id == id)
    {
      return Sockets[i];
    }
  }
  //return NULL;
}

bool isNegativeLogic(int outputPin, int inputPin)
{
  int outputPinStatus = digitalReadOutputPin(outputPin);
  int inputPinStatus = digitalRead(inputPin);
  
  return outputPinStatus != inputPinStatus;
}

int digitalReadOutputPin(uint8_t pin)
{
  uint8_t bit = digitalPinToBitMask(pin);
  uint8_t port = digitalPinToPort(pin);
  
  if (port == NOT_A_PIN)
  {
   return LOW;
  }

  return (*portOutputRegister(port) & bit) ? HIGH : LOW;
}
