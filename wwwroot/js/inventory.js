document.querySelectorAll(".admin-table tbody tr").forEach((row, index) => {
  row.style.animationDelay = `${index * 24}ms`;
  row.classList.add("fade-in");
});
