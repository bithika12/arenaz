const moment = require('moment')
//const momenttz = require('moment-timezone')
const timeZone = 'Asia/Calcutta';



let currentTimestamp = () =>{
    return  moment(new Date())
}
let now = () => {
  return moment.utc().format()
}

let getLocalTime = () => {
  return moment().tz(timeZone).format()
}

let convertToLocalTime = (time) => {
  return momenttz.tz(time, timeZone).format('LLLL')
}
let timeDiffernce = (time) => {
  return momenttz.tz(time, timeZone).format('LLLL')
}

module.exports = {
  now                : now,
  getLocalTime       : getLocalTime,
  convertToLocalTime : convertToLocalTime,
  currentTimestamp   : currentTimestamp
}