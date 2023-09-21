const { getSubtitles } = require('youtube-caption-extractor')

async function downloadYouTubeCaptions(url, lang = 'en') {
  try {
    const videoId = extractVideoId(url)
    const videoCaptions = await getSubtitles({
      videoID: videoId,
      lang: lang
    })
    let formattedCaptions = ''
    videoCaptions.forEach((caption) => {
      // round the seconds to the nearest second
      caption.start = Math.round(caption.start)
      const formattedTime = formatTime(caption.start)
      formattedCaptions += `Timestamp: ${formattedTime}, Caption: ${caption.text}\n\n`
    })
    return formattedCaptions
  } catch (error) {
    return { status: 500, body: error.message }
  }
}

function extractVideoId(url) {
  const regex = /(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/.*[&?#]v=)([\w-]{11})/
  const match = url.match(regex)
  return match ? match[1] : null
}

function formatTime(seconds) {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = Math.floor(seconds % 60)
  const formattedMinutes = String(minutes).padStart(2, '0')
  const formattedSeconds = String(remainingSeconds).padStart(2, '0')
  return `${formattedMinutes}:${formattedSeconds}`
}

module.exports = { downloadYouTubeCaptions }
// export async function DownloadYouTubeCaptionsAsync(url, lang = 'en') {
//   return await downloadYouTubeCaptions(url, lang)
// }
