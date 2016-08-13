<!DOCTYPE html>
<html lang="en" 
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:fb="http://ogp.me/ns/fb#">
	<head>
		<meta name="viewport" content="width=device-width, initial-scale=1" />
		<link rel="icon" href="img/logo.png" type="image/png">
		<meta property="og:image" content="http://hanjo.synology.me/kana-vr/img/logo.png" />
		<meta property="og:type" content="website" />
		<meta name="description" content="A game to memorize the Japanese Hiragana and Katakana letters using a Google Cardboard or similar VR headset. Available on Android and iOS" />
		<meta name="keywords" content="kana, hiragana, katakana, vr, google cardboard, ios, appstore, google play, hanjo, kanavr, kana vr, japanese characters, learning game, languages, japanese" />
		<title>KanaVR</title>
		<style type="text/css">
			html {

			}

			h1 {
				position: absolute;
				top: -10000px;
				left: -10000px;
			}

			body {
				height: 100%;
				background: url(img/background.jpg);
				font-family: Arial, Helvetica, sans-serif;
				font-size: 1em;
				color: #fff;
				position: relative;
				text-align: center;
			}

			#content {
				display: block;
			}

			.shoppingMall {
				width: 100%;
				text-align: center;
			}

			.shoppingMall img {
				height: 3em;
			}

			.download {
				width: 100%;
				clear: both;
			}

			@media (min-width: 541px){
				/* Big view stuff goes here. */
				#content {
					width: 100%;
					vertical-align: middle;
					position: absolute;
					text-align: left;
				}

				#screenshot {
					float: left;
					width: 50%;
					margin: 0 1em 1em 0;
				}

				#logo {
					float: left;
					width: 20%;
					margin: 1.5em 2em 1.5em 1em;
				}

				p {
					margin: 1em;
				}

				.download {
					display: block;
					text-align: center;
				}
			}
			/* and (min-width: 400px) */
			@media (max-width: 540px) {
				/* Smaller view stuff goes here. */
				#screenshot {
					top:0;
					width: 100%;
					display: block;
					position: relative;
				}

				#logo {
					float: left;
					width: 50%;
					margin: 1em .5em .5em 0;
				}

				.download {
					display: none;
				}
			}

		</style>
		<script src="../jquery/jquery.min.js"></script>
		<script>

			$(document).ready(function() {
				updateDownloads();
				setInterval("changeScreenshot()", 10000);
			});

			var imgPath = "./img/scrs/scrs_0";
			var imgExt = ".jpg";
			var currentScrs = 0;
			var scrs = [];
			var isDone = false;

			preloadScreenshots();

			function preloadScreenshots() {
				var maxScrs = 1;
				while (maxScrs < 9) {
					var path = imgPath + maxScrs + imgExt;
					console.log(path);
					$.ajax({
						type: 'POST',
						data: null,
					    url: path,
					    success: function(data){
					    	var img = new Image();
					    	img.src = imgPath + (scrs.length+1) + imgExt;
					    	scrs.push(img);
					        maxScrs++;
					        console.log(scrs.length);
					    },
					    error: function (xhr, options, error) {
					    	console.log(xhr.status);
						    if(xhr.status==404) {
						        isDone = true;
						    }
						}
					});
					maxScrs++;
				}
			}

			function changeScreenshot() {
				currentScrs++;
				if (currentScrs >= scrs.length) {
					currentScrs = 0
				}
				$("#screenshot")[0].src = scrs[currentScrs].src;
			}

			function updateDownloads() {
				var googleButton = $("#googleButton");
				var appleButton = $("#appleButton");

				switch (getMobileOperatingSystem()) {
					case "iOS":
						googleButton.hide();
						break;
					case "Android":
						appleButton.hide();
						break;
				}
			}

			function getMobileOperatingSystem() {
				var userAgent = navigator.userAgent || navigator.vendor || window.opera;

				// Windows Phone must come first because its UA also contains "Android"
				if (/windows phone/i.test(userAgent)) {
					return "Windows Phone";
				}

				if (/android/i.test(userAgent)) {
					return "Android";
				}

				// iOS detection from: http://stackoverflow.com/a/9039885/177710
				if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
					return "iOS";
				}

				return "unknown";
			}

		</script>
	</head>
	<body>
		<div id="content">
			<img src="img/scrs/scrs_01.png" id="screenshot" alt="screenshot" />
			<img src="img/logo.png" id="logo" alt="logo image" />
			<h1>KanaVR</h1>
			<p>Use your VR headset to get familiar with the Japanese Hiragana and Katakana syllable alphabets!</p>
			<p>You are challenged to pick the correct Kana letter before it will hit you.</p>
			<p>A VR headset for smartphones like Google Cardboard or similar will greatly improve the overall experience, even though a non-VR mode exists as well for those without "phoney goggles"</p>
			<p>Most of the text in this app is written and spoken in Japanese for a more authentic feel.</p>
			<h2 class="download">Download</h2>
			<p class="download">You may download KanaVR for your Android or iOS device for free on Google Play or the Apple App Store:</p>
			<div class="shoppingMall">
				<a href="https://play.google.com/store/apps/details?id=com.hanjo.kanavr" id="googleButton" target="_blank">
					<img src="img/google-play-badge.png" alt="Google Play Store" />
				</a>
				<a href="https://appsto.re/ch/AFX9db.i" id="appleButton" target="_blank">
					<img src="img/apple-store-badge.png" alt="Apple App Store" />
				</a>
			</div>
		</div>
	</body>
</html>
