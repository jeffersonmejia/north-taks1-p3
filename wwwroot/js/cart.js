document.querySelectorAll(".add-cart").forEach((button) => {
  button.addEventListener("click", () => button.classList.add("purchase-pulse"));
});
