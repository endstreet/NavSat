const PROXY_CONFIG = [

  {
    context: [
      "/FeatureCollection",
    ],
    target: "https://localhost:5094",
    secure: false
  }
]

module.exports = PROXY_CONFIG;

