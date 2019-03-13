#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import time
import zmq  # pylint: disable=import-error
import math


def FindElementByCoord(x, z):
    return int(z*40+x)


def FindCoordByElement(element):
    tempY = int(element/40)
    tempX = int(element - (40*tempY))
    return tempX, tempY


class tile():
    def __init__(self, x, y, cost):
        self.x = x
        self.y = y
        self.cost = int(cost)
        self.f = 0
        self.g = 0
        self.h = 0
        self.neighbors = None
        self.previousTile = None


def removeElementFromArray(arr, elmt):
    for x in range(arr.__len__()-1, -1, -1):
        if arr[x] == elmt:
            arr.pop(x)
    return arr


def getNeighbors(tile, allTiles):
        # If this position (tile) doesn't have neighbors defined
    if tile.neighbors is None:
        tile.neighbors = findNeighbors(tile, allTiles)
    return tile.neighbors


def findNeighbors(tile, allTiles):
    tile.neighbors = []
    # Go through the tiles of the map
    # UP
    # if tile's y is on cieling
    # print("X and Y of tile: ", tile.x, ", ", tile.y)
    tileElement = FindElementByCoord(tile.x, tile.y)
    if tile.y is not allTiles[(allTiles.__len__()-1)].y:
        tile.neighbors.append(allTiles[(tileElement+40)])

    # RIGHT
    # if tile's x is on the right side
    if tile.x is not allTiles[(allTiles.__len__()-1)].x:
        tile.neighbors.append(allTiles[(tileElement+1)])

    # DOWN
    # if tile's y is on floor
    if tile.y is not 0:
        tile.neighbors.append(allTiles[(tileElement-40)])

    # LEFt
    # if tile's x is on the left side
    if tile.x is not 0:
        tile.neighbors.append(allTiles[(tileElement-1)])

    return tile.neighbors


def Heuristic(posA, posB):
    # returns distance between two given points
    return (math.sqrt((posB.x - posA.x)**2 + (posB.y - posA.y)**2))


def findPathTaken(currPosition):
    # tempReturn = "1:"
    returnString = ""
    currTile = currPosition
    returnString += str(FindElementByCoord(currTile.x, currTile.y)) + ";"
    # Iteravily goes through the tiles (backwards), appends this to returnString 
    while currTile.previousTile is not None:
        currTile = currTile.previousTile
        returnString += str(FindElementByCoord(currTile.x, currTile.y)) + ";"
    #Removes last caracter
    return returnString[:-1]


# AgentPosition : Int,     EndGoal : Int,    tiles: Int[]    (COST)
def FindRouteForAgent(AgentPosition, endGoal, tiles):
    allTiles = []
    for x in range(0, tiles.__len__()):
        tempX, tempY = FindCoordByElement(x)
        allTiles.append(tile(tempX, tempY, tiles[x]))

    startPosition = allTiles[AgentPosition]
    print("Start position: ", startPosition.x, ", ", startPosition.y)

    endGoalPosition = allTiles[endGoal]
    print("End position: ", endGoalPosition.x, ", ", endGoalPosition.y)

    closedArray = []
    openArray = []
    openArray.append(startPosition)

    while openArray.__len__() != 0:
        lowestFElement = 0
        for x in range(0, openArray.__len__()):
            # if found element f is lower than previous found lowest
            # set lowest F Element to be equal to new found element
            if openArray[x].f < openArray[lowestFElement].f:
                lowestFElement = x

            # if there is a tie between found element's f
            # and previous found lowest F element
            if openArray[x].f == openArray[lowestFElement].f:
                # it should then prefer to explore options closer to goal
                if openArray[x].g > openArray[lowestFElement].g:
                    lowestFElement = x

        # first time program is run, the currentTile element will be set to startposition
        currentPosition = openArray[lowestFElement]
        print("Current position: ", currentPosition.x, ",", currentPosition.y)

        # check if the goal was found on the current Tile
        if currentPosition == endGoalPosition:
            print("GOAL HAS BEEN REACHED")
            # Find and return the path taken for this agent
            return findPathTaken(currentPosition)
        # Remove currentPosition from openArray and pushes it to closedArray
        openArray = removeElementFromArray(openArray, currentPosition)
        closedArray.append(currentPosition)

        # Find the neighbors of currentPosition's tile
        neighborsArr = getNeighbors(currentPosition, allTiles)

        # For each of these, find the best approach towards goal
        for x in range(0, neighborsArr.__len__()):
            currNeighbor = neighborsArr[x]

            # Is this a valid next spot?
            # if the currr neighbor has not already been walked through
            if currNeighbor not in closedArray:
                tempG = currentPosition.g + \
                    allTiles[FindElementByCoord(
                        currNeighbor.x, currNeighbor.y)].cost
                print("The temp G for X;Y", currNeighbor.x, ",", currNeighbor.y, ".  Temp g: ", tempG)
                # tempG = currentPosition.g + Heuristic(currNeighbor, currentPosition)
                # Is this G better than previously calculated?
                if currNeighbor not in openArray:
                    openArray.append(currNeighbor)
                elif tempG >= currNeighbor.g:
                    # it is not a better path.
                    # continue trough the other neighbors (doesn't break for-loop)
                    continue

                # Better neighbor has been found
                # Saves this neighbors findings' in its own tiles g,h, and f variables
                currNeighbor.g = tempG
                currNeighbor.h = Heuristic(currNeighbor, endGoalPosition)

                currNeighbor.f = currNeighbor.g + currNeighbor.f
                currNeighbor.previousTile = currentPosition
        # currNeighbor is after the for loop, the best approach from currentPosition
    pass


print("Server starting...")
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

print("Server started")
_allTiles = None

# Python runs this while loop until it dies
while True:
    # Wait for next request from client
    message = socket.recv()
    # The first message sent from the server (UNITY) is all the tiles,
    # some formatting is needed
    msgType = message.decode().split(':')

    # Instantiates the string that will be returned to UNITY
    sndMsg = ""
    sendType = 0
    # If msgType is == to 0, it means that UNITY has sent all the tiles generated
    if (msgType[0] == "0"):
        sendType = 0
        # msgType[1].split(';') returns an 1D array that is saved in _allTiles
        _allTiles = msgType[1].split(';')
        print("Printing all tiles taken from server")
        print(_allTiles)
    # If msgType is == 1, it means that UNITY has sent the destination and the agents' current positions
    elif (msgType[0] == "1"):
        sendType = 1
        # Destination element ID
        destination = msgType[1].split(",")[0]
        # Array of agent positions
        agentPositions = msgType[1].split(",")[1].split(";")

        print("Printing target position:", destination)
        print("Printing all the agents positions: ", agentPositions)
        for item in agentPositions:
            sndMsg += FindRouteForAgent(int(item), int(destination), _allTiles)
            sndMsg += ':'
    else:
        print("Couldn't understand your request")

    # Based on what data racieved, python will send something back to UNITY
    if sendType is 0:
        socket.send(b"Python recieved the tiles!")
    if sendType is 1:
        socket.send_string(sndMsg[:-1])