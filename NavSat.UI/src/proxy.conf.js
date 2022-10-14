const PROXY_CONFIG = [

  {
    context: [
      "/FeatureCollection",
    ],
    target: "http://localhost:5091/api/satellitepath/geolocations",
    secure: false
  }
]

module.exports = PROXY_CONFIG;

