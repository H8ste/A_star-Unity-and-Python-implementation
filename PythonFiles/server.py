#
#   A* Implementation in Python
#   Binds REP socket to tcp://*:5555
#   Expects message from client, replies based on message recieved
#

import time
import zmq 
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
    tileElement = FindElementByCoord(tile.x, tile.y)

    # UP
    # The tile doesn't have a neighbor above, if it's y-value is the same as the max
    if tile.y is not allTiles[(allTiles.__len__()-1)].y:
        tile.neighbors.append(allTiles[(tileElement+40)])

    # RIGHT
    # The tile doesn't have a neighbor to the right, if it's x-value is the same as the max
    if tile.x is not allTiles[(allTiles.__len__()-1)].x:
        tile.neighbors.append(allTiles[(tileElement+1)])

    # DOWN
    # The tile doesn't have a neighbor below, if it's y-value is the same as the lowest possible (0)
    if tile.y is not 0:
        tile.neighbors.append(allTiles[(tileElement-40)])

    # LEFT
    # The tile doesn't have a neighbor to the left, if it's x-value is the same as the lowest possible (0)
    if tile.x is not 0:
        tile.neighbors.append(allTiles[(tileElement-1)])

    # Return the found neighbors
    return tile.neighbors


def Heuristic(posA, posB):
    # Returns distance between two given points
    return (math.sqrt((posB.x - posA.x)**2 + (posB.y - posA.y)**2))


def findPathTaken(currPosition):
    returnString = ""
    currTile = currPosition
    returnString += str(FindElementByCoord(currTile.x, currTile.y)) + ";"
    # Iteravily goes through the tiles (backwards), appends this to returnString 
    while currTile.previousTile is not None:
        currTile = currTile.previousTile
        returnString += str(FindElementByCoord(currTile.x, currTile.y)) + ";"
    # Removes last caracter
    return returnString[:-1]


# AgentPosition : Int,     EndGoal : Int,    tiles: Int[]    (COST)
def FindRouteForAgent(AgentPosition, endGoal, tiles):
    # Defines a array containing tile objects representing the tiles
    #   generated on the Unity-side with their respective Cost and placements
    allTiles = []
    for x in range(0, tiles.__len__()):
        tempX, tempY = FindCoordByElement(x)
        allTiles.append(tile(tempX, tempY, tiles[x]))

    # Sets the start position and end position based on input
    startPosition = allTiles[AgentPosition]
    endGoalPosition = allTiles[endGoal]

    # Instantiates the closedArray - often called the closedSet
    closedArray = []
    # Instantiates the openArray - often called the openSet
    openArray = []
    # And inserts the agents "node" into the openSet
    openArray.append(startPosition)

    # Initially check if the start position is not already the goal
    if openArray[0] == endGoalPosition:
        # Find and return the path taken for this agent
        return str(FindElementByCoord(openArray[0].x, openArray[0].y))

    # If there is anything in the openSet
    #   meaning there is anything left to compute
    while openArray.__len__() != 0:
        #find the lowest element in the openset
        lowestFElement = 0



        for index in range(0, openArray.__len__()):
            # If found element f is lower than previous found lowest
            # set lowest f Element to be equal to the new found element
            if openArray[index].f < openArray[lowestFElement].f:
                lowestFElement = index

            # If there is a tie between found element's f
            # and previous found lowest F element
            if openArray[index].f == openArray[lowestFElement].f:
                # It should then prefer to explore options closer to goal
                if openArray[index].g > openArray[lowestFElement].g:
                    lowestFElement = index

        # Set the currentPosition equal to the tile within the 
        #   openSet with the lowest f value
        # As their is only one element within the openSet to begin with
        #   the Agent's position will be inserted the first time around
        currentPosition = openArray[lowestFElement]

        # Before it computes the paths next nodes f value, it check if
        #   the goal is the current node
        if currentPosition == endGoalPosition:
            # Find and return the path taken for this agent
            return findPathTaken(currentPosition)

        # As the lowest found element of openSet was not the goal
        #   remove it from the openSet and put it into the closedSet
        #       so its not used in future computations
        openArray = removeElementFromArray(openArray, currentPosition)
        closedArray.append(currentPosition)

        # Find the neighbors of currentPosition's tile
        neighborsArr = getNeighbors(currentPosition, allTiles)

        # Find the neighbor with the lowest g value, 
        #   and compute the f value for that neighbor
        for x in range(0, neighborsArr.__len__()):
            currNeighbor = neighborsArr[x]

            # If neighbor has not already been walked through
            if currNeighbor not in closedArray:
                # Cost from start to the neighbor
                tempG = currentPosition.g + \
                    allTiles[FindElementByCoord(
                        currNeighbor.x, currNeighbor.y)].cost

                # Is this cost better than previously calculated?
                if currNeighbor not in openArray:
                    openArray.append(currNeighbor)
                elif tempG >= currNeighbor.g:
                    # It is not a better path.
                    # Continue trough the other neighbors (doesn't break for-loop)
                    #   but skips the following 4 lines of code
                    continue

                # Better neighbor, than previously found, has been found
                # Saves this neighbors findings' in its own tiles g,h, and f variables
                currNeighbor.g = tempG
                currNeighbor.h = Heuristic(currNeighbor, endGoalPosition)

                currNeighbor.f = currNeighbor.g + currNeighbor.h
                # Sets the parent node for this neighbor to the current node 
                currNeighbor.previousTile = currentPosition
    pass

#
# This is the program's start of run-time
#

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
    
    # msgType can be either 0 or 1
    # If msgType is == to 0, it means that UNITY has sent all the tiles generated
    if (msgType[0] == "0"):
        sendType = 0
        # msgType[1].split(';') returns an 1D array that is saved in _allTiles
        _allTiles = msgType[1].split(';')
    # If msgType is == 1, it means that UNITY has sent the goal and the agents' current positions
    elif (msgType[0] == "1"):
        sendType = 1
        # Goal element ID
        goal = msgType[1].split(",")[0]
        # Array of agent positions
        agentPositions = msgType[1].split(",")[1].split(";")

        # For each recieved agent, computes the lowest cost path to the goal 
        # from its start position
        for agent in agentPositions:
            sndMsg += FindRouteForAgent(int(agent), int(goal), _allTiles)
            sndMsg += ':'
    else:
        print("Couldn't understand message sent by Unity")

    # Based on what data recieved, python will send something back to UNITY
    if sendType is 0:
        socket.send(b"Python recieved the tiles")
    if sendType is 1:
        socket.send_string(sndMsg[:-1])
    # Go through while statement again, to check for new message(s)