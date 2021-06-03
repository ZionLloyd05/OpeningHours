# Opening Hours Service Docs

### Part 1
#### Techology used
1. .Net Core 5.0
2. Visual Studio 2019 IDE
3. Postman

#### How to Project
1. Confirm that nugget packages are installed
2. Click on the build icon ▶ or use `dotnet run` command
3. Inside is project is a **testSchedule.json** for test purpose
4. Inclusive in the project is also a **OpenHours.postman_collection.json**
file, this should be imported in postman for an accurate setting.
5. The end result should look like the image below
![result](https://res.cloudinary.com/bookstand/image/upload/v1622706740/result_s0fy6a.png)


### Part 2
#### How to best store the data
This current pattern makes it such that two loops has to be created, the first to loop through
the day, and the various opening hours. 
But for performance sake, the day can also be included has part of the inner loop property.
E.g.

`{
    "day" : "monday",
    "type" : "open",
    "hour" : "34200"
 },
{
    "day" : "monday",
    "type" : "close",
    "hour" : "34200"
}
`

This way in my opinion, there would not be a need for double loops, more like 
flattening up the data and all operation can be done in a linear time, In my opinion.

#### Few Assumption made
1. Any schedule that ends with open, will always have it's close type in the next schedule