const images = [
    "/Image/Главное.png",
    "/Image/Главное3.png"
];

let currentImageIndex = 0;
const mainImage = document.getElementById('mainImage');

// Check minimum window size
function checkWindowSize() {
    const minWidth = 320;
    const minHeight = 480;

    if (window.innerWidth < minWidth || window.innerHeight < minHeight) {
        alert(`Для корректного отображения требуется минимальное разрешение ${minWidth}x${minHeight} пикселей.`);
    }
}

// Image change handler
function changeImage(direction) {
    currentImageIndex += direction;

    if (currentImageIndex < 0) {
        currentImageIndex = images.length - 1;
    } else if (currentImageIndex >= images.length) {
        currentImageIndex = 0;
    }

    mainImage.src = images[currentImageIndex];
}

// Event listeners
document.addEventListener('keydown', function (event) {
    if (event.key === 'ArrowLeft') {
        changeImage(-1);
    } else if (event.key === 'ArrowRight') {
        changeImage(1);
    }
});

window.addEventListener('load', checkWindowSize);
window.addEventListener('resize', checkWindowSize);

// Error handling
mainImage.onerror = function () {
    console.error('Ошибка загрузки изображения:', this.src);
};