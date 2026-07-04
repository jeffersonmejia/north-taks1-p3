document.querySelectorAll(".glass-card").forEach((card, index) => {
  card.style.animationDelay = `${index * 45}ms`;
});
