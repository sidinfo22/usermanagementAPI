const express = require("express");
const bodyParser = require("body-parser");
const morgan = require("morgan");

const app = express();
const port = 3000;

// Middleware for logging and parsing
app.use(bodyParser.json());
app.use(morgan("dev"));

// In-memory user storage (replace with a database in production)
let users = [];

// Validation middleware
function validateUser(req, res, next) {
  const { name, email } = req.body;
  if (!name || !email) {
    return res.status(400).json({ error: "Name and email are required" });
  }
  next();
}

// CRUD Endpoints

// GET all users
app.get("/users", (req, res) => {
  res.json(users);
});

// GET a single user by ID
app.get("/users/:id", (req, res) => {
  const user = users.find((u) => u.id === parseInt(req.params.id));
  if (!user) {
    return res.status(404).json({ error: "User not found" });
  }
  res.json(user);
});

// POST a new user
app.post("/users", validateUser, (req, res) => {
  const newUser = {
    id: users.length + 1,
    name: req.body.name,
    email: req.body.email,
  };
  users.push(newUser);
  res.status(201).json(newUser);
});

// PUT to update a user
app.put("/users/:id", validateUser, (req, res) => {
  const user = users.find((u) => u.id === parseInt(req.params.id));
  if (!user) {
    return res.status(404).json({ error: "User not found" });
  }
  user.name = req.body.name;
  user.email = req.body.email;
  res.json(user);
});

// DELETE a user
app.delete("/users/:id", (req, res) => {
  const userIndex = users.findIndex((u) => u.id === parseInt(req.params.id));
  if (userIndex === -1) {
    return res.status(404).json({ error: "User not found" });
  }
  users.splice(userIndex, 1);
  res.status(204).send();
});

// Error handling middleware
app.use((err, req, res, next) => {
  console.error(err.stack);
  res.status(500).send({ error: "Something went wrong!" });
});

app.listen(port, () => {
  console.log(`User Management API running on http://localhost:${port}`);
});
