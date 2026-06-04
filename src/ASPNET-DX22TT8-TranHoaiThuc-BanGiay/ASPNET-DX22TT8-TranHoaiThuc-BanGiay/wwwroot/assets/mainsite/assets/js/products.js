function setView(type) {
  const grid = document.getElementById("productGrid");
  const gBtn = document.getElementById("grid-btn");
  const lBtn = document.getElementById("list-btn");
  if (type === "list") {
    grid.classList.add("list-view");
    lBtn.classList.add("active");
    gBtn.classList.remove("active");
  } else {
    grid.classList.remove("list-view");
    gBtn.classList.add("active");
    lBtn.classList.remove("active");
  }
}
function clearFilters() {
  document
    .querySelectorAll(".active-filters .filter-tag")
    .forEach((t) => t.remove());
  document
    .querySelectorAll('input[type="checkbox"]')
    .forEach((c) => (c.checked = false));
  document
    .querySelectorAll(".size-btn")
    .forEach((b) => b.classList.remove("active"));
  document
    .querySelectorAll(".color-swatch")
    .forEach((s) => s.classList.remove("active"));
}
document.querySelectorAll(".color-swatch").forEach((s) => {
  s.addEventListener("click", () => {
    s.classList.toggle("active");
  });
});
document.querySelectorAll(".size-btn").forEach((b) => {
  b.addEventListener("click", () => {
    b.classList.toggle("active");
  });
});
